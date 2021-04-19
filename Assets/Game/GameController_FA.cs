using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameController_FA : MonoBehaviourPun
{
    private float currentTime = 300f;
    private UIController_FA UI_controller;

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

    private void Update()

    {
        if (!photonView.IsMine) return;
    
        //if (!isGameOn) return;
        Debug.Log("entro aca????");
        currentTime -= Time.deltaTime;
    
        int minutes = Mathf.FloorToInt(currentTime / 60F);
        int seconds = Mathf.FloorToInt(currentTime - minutes * 60);
    
        if(UI_controller != null)
            UI_controller.RefreshTime(string.Format("Time: {0:0}:{1:00}", minutes, seconds));
    
        // if (currentTime <= 0)
        // {
        //     //FinishGame();
        // }
    }
}
