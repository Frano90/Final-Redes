using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RatCharacter_FA : Character_FA
{
    private GameItem_DATA itemCarrying;    
    
    [SerializeField]private ItemPickerView pickerContainer;
    
    private bool _carryngItem;
    
    public bool IsCarryingItem => _carryngItem;
    
    public int lives { get; private set; }
    [SerializeField] private int startingLifes;
    
    public override Character_FA SetInitialParameters(Player localPlayer, Vector3 startingPos)
    {
        base.SetInitialParameters(localPlayer, startingPos);
        lives = startingLifes;
        photonView.RPC("RPC_SetItemPickerViewer", RpcTarget.OthersBuffered);
        return this;
    }
    
    public void GetTrapped()
    {
        lives--;

        if (lives <= 0)
        {
            lives = 0;
            MyServer_FA.Instance.gameController.PlayerWithoutLives(_owner);
        }
        
        ResetCharacter();
    }
    
    [PunRPC]
    protected void RPC_SetItemPickerViewer()
    {
        pickerContainer.SetItemRegistry(FindObjectOfType<GameController_FA>());
    }
    
    public void PickUpItem(GameItem_DATA itemData)
    {
        _carryngItem = true;
        itemCarrying = itemData;
        photonView.RPC("RPC_PickItemView", RpcTarget.Others, itemData.type);
    }
    
    [PunRPC]
    protected void RPC_PickItemView(GameItem_DATA.ItemType itemData)
    {
        pickerContainer.SetCurrentModel(itemData);
    }
    
    public void ReleaseItem()
    {
        _carryngItem = false;
        itemCarrying = null;
        photonView.RPC("RCP_ReleaseItemView", RpcTarget.Others);
    }
    
    [PunRPC]
    void RCP_ReleaseItemView()
    {
        pickerContainer.ReleaseItem();
    }
    
}
