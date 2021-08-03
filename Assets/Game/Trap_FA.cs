using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Trap_FA : GameItem_FA
{
    private Animator _anim;

    [SerializeField] private ParticleSystem getTrappedRat;
    
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
        
        photonView.RPC("RPC_TrappedRatFeedback", RpcTarget.Others);
        
        MyServer_FA.Instance.gameController.RatTrapped(player);
    }

    [PunRPC]
    void RPC_TrappedRatFeedback()
    {
        if(getTrappedRat != null) getTrappedRat.Play();
    }

    void DelayTrapReset()
    {
        triggered = false;
    }
}
