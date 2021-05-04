using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ItemReceiver_FA : MonoBehaviourPun
{
    
    [SerializeField] private LayerMask triggerLayers;
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        if ((triggerLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            Character_FA player = other.gameObject.GetComponent<Character_FA>();

            if (player.IsCarryingItem)
            {
                MyServer_FA.Instance.gameController.CashItemFromPlayer(player._owner);
            }
                
        }
    }
}
