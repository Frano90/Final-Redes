using System;
using System.Collections;
using System.Collections.Generic;
using FranoW;
using FranoW.DevelopTools;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameController_FA : MonoBehaviourPun
{
    private float currentTime = 300f;
    private UIController_FA UI_controller;


    public int CheeseRecoveredAmount { get; private set; }  
    
    Dictionary<Player, Character_FA> _dicModels = new Dictionary<Player, Character_FA>();
    //Dictionary<Player, GameItem_DATA> _dicCarryItems = new Dictionary<Player, GameItem_DATA>();

    [SerializeField] private List<GameItem_DATA> gameItems = new List<GameItem_DATA>();

    public List<GameItem_DATA> GetGameItems => gameItems;
    public int CheeseAmountToWin { get; set; }


    public bool IsGameFinished()
    {
        return true;
    }

    private void Awake()
    {
        if (!photonView.IsMine) return;
        
        UI_controller = FindObjectOfType<UIController_FA>();
    }

    private void Start()
    {
        if (!photonView.IsMine) return;

        CheeseAmountToWin = 1;
        currentTime = 300f;
    }

    public void AddModel(Player player, Character_FA character)
    {
        if (!_dicModels.ContainsKey(player))
        {
            _dicModels.Add(player, character);
        }
    }

    public void OnTrapTrigger(Player player)
    {
        _dicModels[player].ResetCharacter(Vector3.zero);
        UI_controller.RatTrapped(player);
    }
    
    private void Update()

    {
        if (!photonView.IsMine) return;
        
        HandleGameTime();
    }

    private void HandleGameTime()
    {
        currentTime -= Time.deltaTime;

        int minutes = Mathf.FloorToInt(currentTime / 60F);
        int seconds = Mathf.FloorToInt(currentTime - minutes * 60);

        if (UI_controller != null)
            UI_controller.RefreshTime(string.Format("Time: {0:0}:{1:00}", minutes, seconds));
    }

    public void OnPickUpGameItem(Player player, GameItem_FA item)
    {
        for (int i = 0; i < gameItems.Count; i++)
        {
            if (gameItems[i].type.Equals(item.itemType))
            {
                _dicModels[player].PickUpItem(gameItems[i]);
                break;
            }
        }
    }

    public void CashItemFromPlayer(Player owner)
    {
        CheeseRecoveredAmount++;
        MyServer_FA.Instance.eventManager.TriggerEvent(GameEvent.cheeeseDelivered);
        _dicModels[owner].ReleaseItem();

        OnCheeseDelivered();
    }

    private void OnCheeseDelivered()
    {
        if(CheeseRecoveredAmount >= CheeseAmountToWin)
        {
            MyServer_FA.Instance.eventManager.TriggerEvent(GameEvent.gameFinished);
            MyServer_FA.Instance.ReloadLobby();
        }
            
    }
}
