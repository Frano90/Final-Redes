using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class LobbyController_FA : MonoBehaviourPun
{
    public CharSelect_FA[] characterSelections;

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
    
    public void RegisterLocalData(Player player, int playerIndex)
    {
        playerRegistry.Add(playerIndex, player);
        
        photonView.RPC("RPC_InitializeLocalLobby", player, playerIndex);

        for (int i = 0; i < MyServer_FA.Instance.PlayersConnected; i++)
        {
            if(i.Equals(playerIndex)) continue;

            var data = MyServer_FA.Instance.GetCharacterLobbyDataDictionary[playerRegistry[i]];

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
                //if(j.Equals(i)) continue;
                
                var data = MyServer_FA.Instance.GetCharacterLobbyDataDictionary[playerRegistry[j]];

                for (int k = 0; k < MyServer_FA.Instance.lobySelectorDatas.Count; k++)
                {
                    if (MyServer_FA.Instance.lobySelectorDatas[k].Equals(data))
                    {
                        bool playerIsReady = MyServer_FA.Instance.GetPlayersReadyDictionary[playerRegistry[k]];
                        
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
    

    #endregion
    
    
    void OnPressReadyButton(int index)
    {
        MyServer_FA.Instance.RequestReadyToPlay(PhotonNetwork.LocalPlayer);
        photonView.RPC("RPC_RefreshView",MyServer_FA.Instance.GetServer, index, PhotonNetwork.LocalPlayer);
        
    }
    
    public void SetInitialView(int selectionMenu, Player player)
    {
        photonView.RPC("RPCSetInitialView", RpcTarget.OthersBuffered, selectionMenu, player);
        photonView.RPC("RPCEnableButtons", player, selectionMenu);

    }

    


    
    
    public void SetInitialParams(Player player, int index)
    {
        photonView.RPC("RPCRegisterButtons", RpcTarget.OthersBuffered, index, player);
        SetInitialView(index, player);
    }

    [PunRPC]
    public void RPCRegisterButtons(int index, Player player)
    {
        characterSelections[index].SetInitialParams(index);
    }

    [PunRPC]
    public void RPC_RefreshView(int index, Player player)
    {
        Debug.Log("que es esto?" + MyServer_FA.Instance.GetPlayersReadyDictionary[player]);
        photonView.RPC("RPC_RefreshReadyButton", RpcTarget.OthersBuffered, index, MyServer_FA.Instance.GetPlayersReadyDictionary[player]);
        
    }

    [PunRPC]
    void RPC_RefreshReadyButton(int index, bool playerReady)
    {
        characterSelections[index].ChangeReadyButtonColor(playerReady);
    }
    
    [PunRPC]
    public void RPCSetInitialView(int index, Player player)
    {
        Debug.Log("el index es " + index + " y el nombre del player es " + player.NickName);
        characterSelections[index].SetInitialView(player.NickName, index);
    }

    [PunRPC]
    public void RPCEnableButtons(int index)
    {
        characterSelections[index].ToggleReadyButton(true);
        characterSelections[index].ToggleTeamButton(true);
    }
    
}
