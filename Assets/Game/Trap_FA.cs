using Photon.Realtime;

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
