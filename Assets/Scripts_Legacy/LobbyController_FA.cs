using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class LobbyController_FA : MonoBehaviourPun
{
    public CharSelect_FA[] characterSelections;

    //Dictionary<int, Player> playerRegistry = new Dictionary<int, Player>();

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
                characterSelections[i].onPressed_Ready_btt = (index) => MyServer_FA.Instance.RequestReadyToPlay(PhotonNetwork.LocalPlayer);
            }
        }
    }

    public void SetInitialView(int selectionMenu, Player player)
    {
        photonView.RPC("RPCSetInitialView", RpcTarget.OthersBuffered, selectionMenu, player);
        photonView.RPC("RPCEnableButtons", player, selectionMenu);

    }

    void RequestChangeCharacterData(int index)
    {
        photonView.RPC("RPC_ChangeCharacterData", MyServer_FA.Instance.GetServer, index);
    }

    [PunRPC]
    void RPC_ChangeCharacterData(int index)
    {
        photonView.RPC("RPC_SetNextCharacter", RpcTarget.OthersBuffered, index);
    }

    [PunRPC]
    void RPC_SetNextCharacter(int index)
    {
        characterSelections[index].ChangeCharacter();
    }
    
    public void SetInitialParams(Player player, int index)
    {
        //playerRegistry.Add(index, player);
        //characterSelections[index].SetInitialParams(index);
        photonView.RPC("RPCRegisterButtons", RpcTarget.OthersBuffered, index, player);
        SetInitialView(index, player);
    }

    public void RequestRefreshView(int index, Player player, bool isReady)
    {
        
        photonView.RPC("RPCRefreshView",RpcTarget.OthersBuffered, index , isReady);
    }

    [PunRPC]
    public void RPCRegisterButtons(int index, Player player)
    {
        
        characterSelections[index].SetInitialParams(index);
    }

    [PunRPC]
    public void RPCRefreshView(int index, bool isReady)
    {
        characterSelections[index].ChangeReadyButtonColor(isReady);
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
