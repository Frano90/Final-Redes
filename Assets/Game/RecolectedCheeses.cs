using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RecolectedCheeses : MonoBehaviourPun
{
    [SerializeField] private List<GameObject> cheeses = new List<GameObject>();

    public void ShowNextCheese()
    {
        photonView.RPC("RPC_ShowCheese", RpcTarget.Others);
    }

    [PunRPC]

    void RPC_ShowCheese()
    {
        foreach (var cheese in cheeses)
        {
            if(cheese.gameObject.activeInHierarchy) continue;
            
            cheese.SetActive(true);

            break;
        }
    }
}
