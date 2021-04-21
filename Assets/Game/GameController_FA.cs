using System;
using System.Collections;
using System.Collections.Generic;
using FranoW;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameController_FA : MonoBehaviourPun
{
    private float currentTime = 300f;
    private UIController_FA UI_controller;

    Dictionary<Player, Character_FA> _dicModels = new Dictionary<Player, Character_FA>();
    
    public bool IsGameFinished()
    {
        return true;
    }

    private void Start()
    {
        if (!photonView.IsMine) return;
        
        UI_controller = FindObjectOfType<UIController_FA>();
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
        Debug.Log(player);
        Debug.Log(player.NickName);
        _dicModels[player].ResetCharacter(Vector3.zero);
    }
    
    private void Update()

    {
        if (!photonView.IsMine) return;
    
        //if (!isGameOn) return;
        Debug.Log("entro aca????");
        HandleGameTime();

        // if (currentTime <= 0)
        // {
        //     //FinishGame();
        // }
    }

    private void HandleGameTime()
    {
        currentTime -= Time.deltaTime;

        int minutes = Mathf.FloorToInt(currentTime / 60F);
        int seconds = Mathf.FloorToInt(currentTime - minutes * 60);

        if (UI_controller != null)
            UI_controller.RefreshTime(string.Format("Time: {0:0}:{1:00}", minutes, seconds));
    }
}
