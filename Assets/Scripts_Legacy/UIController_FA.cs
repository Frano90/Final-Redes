﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;


public class UIController_FA : MonoBehaviourPun
{
    [SerializeField] TMP_Text time;
    [SerializeField] FinishPanel_UI finishPanel;
    [SerializeField] private Transform playerUIContainer;
    [SerializeField] private List<RatUIViewer> playerUIs = new List<RatUIViewer>();
    [SerializeField] private Dictionary<Player, RatUIViewer> _dicPlayerUis = new Dictionary<Player, RatUIViewer>();
    [SerializeField] private CheeseScoreHandler _cheeseScoreHandler;

    //[SerializeField] private GameObject encounterResolverPanel;
    
    private void Start()
    {
        finishPanel.AddEventToContinueButton(OnLocalPlayerPressContinueButton);
        finishPanel.gameObject.SetActive(false);

        FetchPlayerUI();
        
        if (!photonView.IsMine) return;
        
        StartCoroutine(InitSettings());
    }

    public void OpenEncounterWindow(Player catPlayer, Player ratPlayer)
    {
        photonView.RPC("RPC_OpenEncounterWindow", catPlayer);
        photonView.RPC("RPC_OpenEncounterWindow", ratPlayer);
    }

    // [PunRPC]
    // void RPC_OpenEncounterWindow()
    // {
    //     encounterResolverPanel.SetActive(true);
    // }
    //
    // [PunRPC]
    // void RPC_CloseEncounterWindow()
    // {
    //     encounterResolverPanel.SetActive(false);
    // }
    //
    // public void CloseEncounterWindow()
    // {
    //     
    // }
    
    private void FetchPlayerUI()
    {
        foreach (Transform v in playerUIContainer)
        {
            RatUIViewer ratUI = v.GetComponent<RatUIViewer>();
            if (ratUI != null)
            {
                playerUIs.Add(ratUI);
            }
        }
    }

    IEnumerator InitSettings()
    {
        while (MyServer_FA.Instance.gameController == null)
        {
            yield return new WaitForEndOfFrame();
        }
        photonView.RPC("RPC_RefreshCheeseScore", RpcTarget.Others, 0, MyServer_FA.Instance.gameController.CheeseAmountToWin);

        MyServer_FA.Instance.eventManager.SubscribeToEvent(GameEvent.cheeeseDelivered, RefreshCheeseScore);
        MyServer_FA.Instance.eventManager.SubscribeToEvent(GameEvent.gameFinished, OnFinishGame);
    }

    void OnLocalPlayerPressContinueButton()
    {
        Debug.Log("apreto boton");
        MyServer_FA.Instance.RequestEnterLobbyAgain(PhotonNetwork.LocalPlayer);
    }
    
    private void OnFinishGame()
    {
        MyServer_FA.Instance.eventManager.UnsubscribeToEvent(GameEvent.cheeeseDelivered, RefreshCheeseScore);
        MyServer_FA.Instance.eventManager.UnsubscribeToEvent(GameEvent.gameFinished, OnFinishGame);
        photonView.RPC("RPC_OnFinishGame", RpcTarget.Others);
    }

    [PunRPC]
    void RPC_OnFinishGame()
    {
        finishPanel.gameObject.SetActive(true);
    }

    public void RefreshTime(string newTime)
    {
        photonView.RPC("RPCRefreshTime", RpcTarget.Others, newTime);
    }

    public void RefreshCheeseScore()
    {
        int cheeseCount = MyServer_FA.Instance.gameController.CheeseRecoveredAmount;
        int maxCheese = MyServer_FA.Instance.gameController.CheeseAmountToWin;
        photonView.RPC("RPC_RefreshCheeseScore", RpcTarget.Others, cheeseCount, maxCheese);
    }

    public void OpenWinnerPlate(string winner)
    {
        photonView.RPC("RPCOpenWinnerPlate", RpcTarget.Others, winner);
    }

    [PunRPC]
    public void RPCOpenWinnerPlate(string winner)
    {
        //finishPanel.SetActive(true);
        //winnerTeam.text = winner;
    }

    [PunRPC]
    public void RPCRefreshTime(string newTime)
    {
        time.text = newTime;
    }

    [PunRPC]
    public void RPC_RefreshCheeseScore(int newValueScore, int maxCheese)
    {
        Debug.Log(newValueScore +" " + maxCheese);
        _cheeseScoreHandler.RefreshLocalCheeseScore(newValueScore, maxCheese);
    }


    public void RatTrapped(Player player)
    {
        int playerUIIndex = -1;
        
        for (int i = 0; i < playerUIs.Count; i++)
        {
            if (playerUIs[i].Equals(_dicPlayerUis[player]))
            {
                playerUIIndex = i;
                break;
            }
        }

        if (playerUIIndex < 0)
        {
            Debug.LogError("NO ENCONTRO UNA UI");
            return;
        }
        
        var ratChar = MyServer_FA.Instance.gameController.GetCharactersDic[player].GetComponent<RatCharacter_FA>();
        photonView.RPC("RCP_RefreshMouseLife", RpcTarget.Others, playerUIIndex, ratChar.lives);
    }

    [PunRPC]
    void RCP_RefreshMouseLife(int uiIndex, int ratLives)
    {
        playerUIs[uiIndex].RefreshLifeUI(ratLives);
    }
    
    public void RegisterPlayerUI(Player player)
    {
        LobbySelectorData data = MyServer_FA.Instance.GetCharacterLobbyDataDictionary[player];

        if (data.team == LobbySelectorData.Team.cat) return;
        
        int index = 0;

        for (int i = 0; i < MyServer_FA.Instance.lobySelectorDatas.Count; i++)
        {
            if(MyServer_FA.Instance.lobySelectorDatas[i].team == LobbySelectorData.Team.cat) continue;
            
            if (MyServer_FA.Instance.lobySelectorDatas[i].Equals(data))
            {
                index = i;
                break;
            }
        }

        for (int i = 0; i < playerUIs.Count; i++)
        {
            if(playerUIs[i].IsOcupied) continue;
            
            playerUIs[i].SetOcupied();
            photonView.RPC("RPC_SetPlayerUI", RpcTarget.OthersBuffered, i, index, player);

            _dicPlayerUis.Add(player, playerUIs[i]); 
            break;
        }

        
        
    }

    [PunRPC]
    void RPC_SetPlayerUI(int playerUIIndex, int playerDataIndex, Player player)
    {
        playerUIs[playerUIIndex].SetPlayerUI(MyServer_FA.Instance.lobySelectorDatas[playerDataIndex].portrait, player.NickName, "3");
    }
}
