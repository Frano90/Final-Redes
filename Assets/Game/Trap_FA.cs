using System;
using Photon.Realtime;
using UnityEngine;

public class Trap_FA : GameItem_FA
{
    private Animator _anim;

    private bool triggered;
    
    private void Start()
    {
        _anim = GetComponent<Animator>();
        
        if (!photonView.IsMine) return;
        
        AddEventOnTrigger(OnRatTouchedTrap);
    }

    private void Update()
    {
        if (base.photonView.IsMine)
        {
            if(_anim != null) _anim.SetBool("triggered", triggered);
        }
    }

    void OnRatTouchedTrap(Player player)
    {
        triggered = true;
        
        //if(_anim != null) _anim.Play("closeTrap");

        Invoke("DelayTrapReset", 3f);

        MyServer_FA.Instance.gameController.RatTrapped(player);
    }

    void DelayTrapReset()
    {
        triggered = false;
    }
}
