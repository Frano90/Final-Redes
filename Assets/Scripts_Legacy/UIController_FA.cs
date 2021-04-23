using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;


public class UIController_FA : MonoBehaviourPun
{
    [SerializeField] TMP_Text time;
    //[SerializeField] Text b_score;
    //[SerializeField] Text y_score;
    //[SerializeField] GameObject finishPanel;
    //[SerializeField] Text winnerTeam;
    //[SerializeField] Button lobbyButton;
    //[SerializeField] Text mineAmmo;

    [SerializeField] private CheeseScoreHandler _cheeseScoreHandler;
    
    private void Start()
    {
        if (!photonView.IsMine) return;
        
       
        //photonView.RPC("RPC_RefreshCheeseScore", RpcTarget.Others, 0, MyServer_FA.Instance.gameController.CheeseAmountToWin);
        StartCoroutine(InitSettings());
    }

    IEnumerator InitSettings()
    {
        while (MyServer_FA.Instance.gameController == null)
        {
            yield return new WaitForEndOfFrame();
        }
        photonView.RPC("RPC_RefreshCheeseScore", RpcTarget.Others, 0, MyServer_FA.Instance.gameController.CheeseAmountToWin);
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
    public void RPC_RefreshCheeseScore(int newValueScore, int maxCheese)
    {
        Debug.Log(newValueScore +" " + maxCheese);
        _cheeseScoreHandler.RefreshLocalCheeseScore(newValueScore, maxCheese);
    }


}
