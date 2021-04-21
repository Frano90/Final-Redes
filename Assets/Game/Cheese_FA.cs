using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class Cheese_FA : GameItem_FA
{
    private void Start()
    {
        if (!photonView.IsMine) return;
        
        AddEventOnTrigger(OnRatTouchedCheese);
    }

    void OnRatTouchedCheese(Player player)
    {
        MyServer_FA.Instance.gameController.OnPickUpGameItem(player, this);
        
        gameObject.SetActive(false);
    }
}
