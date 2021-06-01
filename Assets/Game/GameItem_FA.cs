using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameItem_FA : MonoBehaviourPun
{
    public event Action<Player> OnTriggerItem;
    [SerializeField] private LayerMask triggerLayers;
    public GameItem_DATA.ItemType itemType;
    
    private void OnTriggerEnter(Collider other)
    {
        if(!photonView.IsMine) return;
        
        if ((triggerLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            Debug.Log("gameItem class");
            OnTriggerItem?.Invoke(other.gameObject.GetComponent<Character_FA>()._owner);
        }
    }

    public void AddEventOnTrigger(Action<Player> callback){OnTriggerItem += callback;}
    
    public void RemoveEventOnTrigger(Action<Player> callback){OnTriggerItem -= callback;}
}
