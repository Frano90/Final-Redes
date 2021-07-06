using Photon.Realtime;
using UnityEngine;

public class Trap_FA : GameItem_FA
{
    private Animator _anim;
    
    private void Start()
    {
        _anim = GetComponent<Animator>();
        
        if (!photonView.IsMine) return;
        
        AddEventOnTrigger(OnRatTouchedTrap);
    }

    void OnRatTouchedTrap(Player player)
    {
        if(_anim != null) _anim.Play("closeTrap");

        MyServer_FA.Instance.gameController.RatTrapped(player);
    }
}
