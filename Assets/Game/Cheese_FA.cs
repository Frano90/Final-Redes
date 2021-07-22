using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Cheese_FA : GameItem_FA
{
    
    public float xAngle, yAngle, zAngle;


    private void Update()
    {
        if (photonView.IsMine) return;
        
        transform.Rotate(xAngle, yAngle, zAngle, Space.Self);
    }

    private void Start()
    {
        if (!photonView.IsMine) return;
        
        AddEventOnTrigger(OnRatTouchedCheese);
    }

    void OnRatTouchedCheese(Player player)
    {
        if(MyServer_FA.Instance.gameController.GetCharactersDic[player].GetComponent<RatCharacter_FA>().IsCarryingItem) return;

        //var cheeseRecolected = Resources.Load<ParticleSystem>("CheeseRecolected.prefab");
        PhotonNetwork.Instantiate("CheeseRecolected", transform.position, Quaternion.identity).GetComponent<ParticleSystem>();


        MyServer_FA.Instance.gameController.OnPickUpGameItem(player, this);
        
        gameObject.SetActive(false);

        photonView.RPC("RPC_OnTouchedCheese", RpcTarget.Others);
        
        Destroy(gameObject);
    }

    [PunRPC]
    void RPC_OnTouchedCheese()
    {
        gameObject.SetActive(false);
    }
}
