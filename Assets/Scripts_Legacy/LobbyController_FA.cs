using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class LobbyController_FA : MonoBehaviourPun
{
    public CharSelect_FA[] characterSelections;

    [SerializeField] private GameObject notCorrectTeamPanel, startingGamePanel;
    
    Dictionary<int, Player> playerRegistry = new Dictionary<int, Player>();

    private void Start()
    {
        if(photonView.IsMine)
        {
            
        }
        else
        {
            
            
            for (int i = 0; i < characterSelections.Length; i++)
            {
                characterSelections[i].ToggleReadyButton(false);
                characterSelections[i].ToggleTeamButton(false);

                Debug.Log("subscribo eventos");
                characterSelections[i].onPressed_ChangeTeam_btt = (index) => RequestChangeCharacterData(index);
                characterSelections[i].onPressed_Ready_btt = (index) => OnPressReadyButton(index);
            }
        }
    }

    #region NewTry

    void RequestChangeCharacterData(int index)
    {
        photonView.RPC("RPC_ChangeCharacterData", MyServer_FA.Instance.GetServer,
            index, PhotonNetwork.LocalPlayer);
    }
    
    [PunRPC]
    void RPC_ChangeCharacterData(int index, Player player)
    {
        MyServer_FA.Instance.RefreshPlayerLobbyData(player);
        RefreshAllClientsViews();
    }
    
    [PunRPC]
    void RPC_SetNextCharacter(int index)
    {
        characterSelections[index].ChangeCharacter();
    }
    
    //Esto es del lado del server
    public void RegisterLocalData(Player player, int playerIndex)
    {
        playerRegistry.Add(playerIndex, player); //registro al jugador recien conectado 
        
        photonView.RPC("RPC_InitializeLocalLobby", player, playerIndex);//registro local

        for (int i = 0; i < MyServer_FA.Instance.PlayersConnected; i++)//por cada jugador conectado
        {
            if(i.Equals(playerIndex)) continue;

            var data = MyServer_FA.Instance.GetCharacterLobbyDataDictionary[playerRegistry[i]];//consigo la data del jugador

            for (int j = 0; j < MyServer_FA.Instance.lobySelectorDatas.Count; j++)
            {
                if (MyServer_FA.Instance.lobySelectorDatas[j].Equals(data))
                {
                    photonView.RPC("RPC_RefreshOtherClientsViews", player, i, j, player.NickName);
                    break;
                }
            }
        }

        
    }

    public void RefreshAllClientsViews()
    {
        for (int i = 0; i < MyServer_FA.Instance.PlayersConnected; i++)
        {
            Player player = playerRegistry[i];

            for (int j = 0; j < MyServer_FA.Instance.PlayersConnected; j++)
            {
                var data = MyServer_FA.Instance.GetCharacterLobbyDataDictionary[playerRegistry[j]];

                for (int k = 0; k < MyServer_FA.Instance.lobySelectorDatas.Count; k++)
                {
                    if (MyServer_FA.Instance.lobySelectorDatas[k].Equals(data))
                    {
                        bool playerIsReady = MyServer_FA.Instance.GetPlayersReadyDictionary[playerRegistry[j]];
                        
                        photonView.RPC("RPC_UpdateView", player, j, k, playerRegistry[j].NickName, playerIsReady);
                        break;
                    }
                }
            }
        }
    }

    [PunRPC]
    void RPC_UpdateView(int playerIndex, int dataIndex, string playerName, bool isReady)
    {
        characterSelections[playerIndex].UpdateView(dataIndex, playerName, isReady);
    }
    
    [PunRPC]
    void RPC_InitializeLocalLobby(int playerIndex)
    {
        characterSelections[playerIndex].SetInitialParams(playerIndex);
        characterSelections[playerIndex].SetInitialView(PhotonNetwork.LocalPlayer.NickName, playerIndex);
        
        characterSelections[playerIndex].ToggleTeamButton(true);
        characterSelections[playerIndex].ToggleReadyButton(true);
    }

    [PunRPC]
    void RPC_RefreshOtherClientsViews(int playerViewIndex, int playerData, string playerName)
    {
        characterSelections[playerViewIndex].SetInitialView(playerName, playerData);
    }
    
    void OnPressReadyButton(int index)
    {
        MyServer_FA.Instance.RequestReadyToPlay(PhotonNetwork.LocalPlayer);
        photonView.RPC("RPC_RefreshView",MyServer_FA.Instance.GetServer, index, PhotonNetwork.LocalPlayer);
    }
    

    [PunRPC]
    public void RPC_RefreshView(int index, Player player)
    {
        photonView.RPC("RPC_RefreshReadyButton", RpcTarget.Others, index, MyServer_FA.Instance.GetPlayersReadyDictionary[player]);
        photonView.RPC("RPC_RefreshChangeTeamButton", player, index, MyServer_FA.Instance.GetPlayersReadyDictionary[player]);
    }

    [PunRPC]
    void RPC_RefreshReadyButton(int index, bool playerReady)
    {
        characterSelections[index].ChangeReadyButtonColor(playerReady);
    }
    
    [PunRPC]
    void RPC_RefreshChangeTeamButton(int index, bool playerReady)
    {
        characterSelections[index].ToggleTeamButton(!playerReady);
    }

    #endregion

    public void ShowPanel(LobbyPanelType panelType)
    {
        photonView.RPC("RPC_ShowPanel", RpcTarget.Others, panelType);
    }

    [PunRPC]
    void RPC_ShowPanel(LobbyPanelType panelType)
    {
        TurnOffPanels();

        if(panelType == LobbyPanelType.NotCorrectTeams) notCorrectTeamPanel.SetActive(true);
        
        if(panelType == LobbyPanelType.StartingGame) startingGamePanel.SetActive(true);
    }

    void TurnOffPanels()
    {
        notCorrectTeamPanel.SetActive(false);
        startingGamePanel.SetActive(false);
    }
    
    public enum LobbyPanelType
    {
        NotCorrectTeams,
        StartingGame
    }
}
