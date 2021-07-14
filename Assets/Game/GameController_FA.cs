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

    [SerializeField] private Spawners_FA spawner;
    public int CheeseRecoveredAmount { get; private set; }  
    
    Dictionary<Player, Character_FA> _dicModels = new Dictionary<Player, Character_FA>();

    [SerializeField] private CatchEncounterHandler _catchEncounterHandler;
    
    [SerializeField] private List<GameItem_DATA> gameItems = new List<GameItem_DATA>();

    public LobbySelectorData.Team WinnerTeam { get; private set; }
    
    public List<GameItem_DATA> GetGameItems => gameItems;
    public Dictionary<Player, Character_FA> GetCharactersDic => _dicModels;
    public int CheeseAmountToWin { get; set; }

    public bool IsGameFinished()
    {
        return true;
    }

    private void Awake()
    {
        _catchEncounterHandler = GetComponent<CatchEncounterHandler>();

        UI_controller = FindObjectOfType<UIController_FA>();
        
    }

    private void Start()
    {
        if (!photonView.IsMine) return;

        CheeseAmountToWin = 15;
        currentTime = 600f;
    }

    public void AddModel(Player player, Character_FA character)
    {
        if (!_dicModels.ContainsKey(player))
        {
            _dicModels.Add(player, character);
        }
    }

    public void RatTrapped(Player player)
    {
        var capturedRat = _dicModels[player].GetComponent<RatCharacter_FA>();
        capturedRat.GetTrapped();
        UI_controller.RatTrapped(player);

        bool result = AreRatsAlive();

        if (!result)
        {
            //Terminar el juego, gano el gato
            WinnerTeam = LobbySelectorData.Team.cat;
            MyServer_FA.Instance.eventManager.TriggerEvent(GameEvent.gameFinished);
            MyServer_FA.Instance.ReloadLobby();
        }
    }

    private bool AreRatsAlive()
    {
        bool value = false;

        foreach (var player in GetCharactersDic.Keys)
        {
            var posibleRatPlayer = GetCharactersDic[player].GetComponent<RatCharacter_FA>();

            if (posibleRatPlayer != null)
            {
                if (posibleRatPlayer.lives > 0)
                {
                    Debug.Log("todavia quedan vivas");
                    value = true;
                    return value;    
                }
            }
        }
        Debug.Log("se murieron todas");
        return value;
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

        if (currentTime <= 0)
        {
            WinnerTeam = LobbySelectorData.Team.undefined;
            MyServer_FA.Instance.eventManager.TriggerEvent(GameEvent.gameFinished);
            MyServer_FA.Instance.ReloadLobby();
        }
    }

    public void OnPickUpGameItem(Player player, GameItem_FA item)
    {
        for (int i = 0; i < gameItems.Count; i++)
        {
            if (gameItems[i].type.Equals(item.itemType))
            {
                _dicModels[player].GetComponent<RatCharacter_FA>().PickUpItem(gameItems[i]);
                break;
            }
        }
    }

    public void CashItemFromPlayer(Player owner)
    {
        CheeseRecoveredAmount++;
        MyServer_FA.Instance.eventManager.TriggerEvent(GameEvent.cheeeseDelivered);
        _dicModels[owner].GetComponent<RatCharacter_FA>().ReleaseItem();

        OnCheeseDelivered();
    }

    private void OnCheeseDelivered()
    {
        if(CheeseRecoveredAmount >= CheeseAmountToWin)
        {
            WinnerTeam = LobbySelectorData.Team.rat;
            MyServer_FA.Instance.eventManager.TriggerEvent(GameEvent.gameFinished);
            MyServer_FA.Instance.ReloadLobby();
        }
            
    }

    public void PlayerWithoutLives(Player player)
    {
        UI_controller.SetNoLivesPanel(player,true);
        
    }

    public void StartCatchEncounter(Player ratPlayer, Player catPlayer)
    {
        _dicModels[ratPlayer].StopMovement();
        _dicModels[ratPlayer].SetEncounter(true);
        //_dicModels[ratPlayer].SetModelRender(false);
        _dicModels[catPlayer].StopMovement();
        _dicModels[catPlayer].SetEncounter(true);
        //_dicModels[catPlayer].SetModelRender(false);
        

        photonView.RPC("RPC_DisableLocalController", catPlayer);
        photonView.RPC("RPC_DisableLocalController", ratPlayer);
        
        _catchEncounterHandler.ActiveCatchEncounter(ratPlayer, catPlayer);
        
    }

    [PunRPC]
    public void RPC_DisableLocalController()
    {
        spawner.GetLocalController.DisableInputs();
    }
    [PunRPC]
    public void RPC_EnableLocalController()
    {
        spawner.GetLocalController.EnableInputs();
    }


    IEnumerator WaitToMouseFlee(Player cat)
    {
        yield return new WaitForSeconds(2);
        ExitEncounter(cat);
    }
    
    public void EncounterFeedbackResult(bool catCatchMouse, Player cat, Player mouse)
    {
        if (catCatchMouse)
        {
            RatTrapped(mouse);
            ExitEncounter(cat);
        }
        else
        {
            StartCoroutine(WaitToMouseFlee(cat));
        }
        
        ExitEncounter(mouse);
    }

    void ExitEncounter(Player player)
    {
        _dicModels[player].SetModelRender(player,true);
        _dicModels[player].ResumeMovement();
        _dicModels[player].SetEncounter(false);
        photonView.RPC("RPC_EnableLocalController", player);
    }
}
