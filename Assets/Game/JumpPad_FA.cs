using Photon.Realtime;
using UnityEngine;

public class JumpPad_FA : GameItem_FA
{
    [SerializeField] private float force;
    [SerializeField] private Vector3 dir;

    [SerializeField] private bool useObjectForward;
    
    private void Start()
    {
        if (!photonView.IsMine) return;
        
        AddEventOnTrigger(OnRatTouchedPad);
    }

    void OnRatTouchedPad(Player player)
    {
        var rat = MyServer_FA.Instance.gameController.GetCharactersDic[player];
        
        var impactReceiver = rat.GetComponent<ImpactReceiver>();
            
        if(useObjectForward)
            impactReceiver.AddImpact((rat.transform.forward + dir).normalized, force);
        else
            impactReceiver.AddImpact((dir).normalized, force);
    }  
}