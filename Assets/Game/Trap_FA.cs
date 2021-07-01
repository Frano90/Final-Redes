using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Trap_FA : GameItem_FA
{
    private void Start()
    {
        if (!photonView.IsMine) return;
        
        AddEventOnTrigger(OnRatTouchedTrap);
    }

    void OnRatTouchedTrap(Player player)
    {
        MyServer_FA.Instance.gameController.RatTrapped(player);
    }
}
