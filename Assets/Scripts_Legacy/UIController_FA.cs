using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


public class UIController_FA : MonoBehaviourPun
{
    [SerializeField] Text time;
    //[SerializeField] Text b_score;
    //[SerializeField] Text y_score;
    //[SerializeField] GameObject finishPanel;
    //[SerializeField] Text winnerTeam;
    //[SerializeField] Button lobbyButton;
    //[SerializeField] Text mineAmmo;

    private void Start()
    {
        if(!photonView.IsMine)
        {
            //Debug.Log("esto se registra");
            //lobbyButton.onClick.AddListener(() => MyServer_FA.Instance.RequestEnterLobbyAgain(PhotonNetwork.LocalPlayer));
        }
            
    }

    public void RefreshTime(string newTime)
    {
        photonView.RPC("RPCRefreshTime", RpcTarget.Others, newTime);
    }

    public void RefreshScore(int blueScore, int yellowScore)
    {
        photonView.RPC("RPCRefreshScore", RpcTarget.Others, blueScore, yellowScore);
    }

    public void OpenWinnerPlate(string winner)
    {
        photonView.RPC("RPCOpenWinnerPlate", RpcTarget.Others, winner);
    }

    public void RefreshMine(int cant)
    {
        //mineAmmo.text = cant.ToString();
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
    public void RPCRefreshScore(int b, int y)
    {
       // b_score.text = b.ToString();
        //y_score.text = y.ToString();
    }
}
