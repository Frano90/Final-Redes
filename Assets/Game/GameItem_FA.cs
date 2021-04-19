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
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("algo me toco");

        
        
        if ((triggerLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            Debug.Log("la rata me tocoooooooo");
            OnTriggerItem?.Invoke(other.gameObject.GetComponent<Character_FA>()._owner);
        }
    }

    public void AddEventOnTrigger(Action<Player> callback)
    {
        OnTriggerItem += callback;
    }
    
    public void RemoveEventOnTrigger(Action<Player> callback)
    {
        OnTriggerItem -= callback;
    }
}
