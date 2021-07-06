using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class JumpPad : MonoBehaviour//GameItem_FA
{
    [SerializeField] private LayerMask triggerLayers;
    [SerializeField] private float force;
    [SerializeField] private Vector3 dir;

    [SerializeField] private bool useObjectForward;
    private void OnTriggerEnter(Collider other)
    {
        if ((triggerLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            Debug.Log("gameItem class");
            var rat = other.GetComponent<ImpactReceiver>();
            
            if(useObjectForward)
                rat.AddImpact((other.transform.forward + Vector3.up * 2).normalized, force);
            else
                rat.AddImpact((dir).normalized, force);
            
            
            //OnTriggerItem?.Invoke(other.gameObject.GetComponent<Character_FA>()._owner);
        }
    }
    // private void Start()
    // {
    //     if (!photonView.IsMine) return;
    //
    //     AddEventOnTrigger(OnRatTouchedJumpPad);
    // }
    //
    // void OnRatTouchedJumpPad(Player player)
    // {
    //     
    // }
}
