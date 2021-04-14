using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MyServer_FA;
//using Photon.Realtime;
//using Photon.Pun;

public class CharSelect_FA : MonoBehaviour
{
    //public Player _owner = null;

    [SerializeField] Image character_Image;
    [SerializeField] Sprite[] imagesPool;

    [SerializeField] Image teamColor_Image;
    //[SerializeField] Button changeTeam_btt;
    [SerializeField] Button ready_btt;
    [SerializeField] Text nickname_text;

    public Action<int> onPressed_ChangeTeam_btt;
    public Action<int> onPressed_Ready_btt;

    //public Team currentTeamSelected = Team.NotAssigned;
    public int playerIndex;

    public bool imReady = false;

    public CharSelect_FA SetInitialParams(int index)
    {
        playerIndex = index;
        Debug.Log("suscribo eventos en selector");
        //changeTeam_btt.onClick.AddListener(() => OnPressed_changeTeam(playerIndex));
        ready_btt.onClick.AddListener(() => OnPressed_ready(playerIndex));


        return this;
    }

    void OnPressed_changeTeam(int index) {onPressed_ChangeTeam_btt?.Invoke(index); Debug.Log("se invoca en el selector"); }
    void OnPressed_ready(int index) => onPressed_Ready_btt?.Invoke(index);

    public void SetInitialView(string playerName)
    {
        nickname_text.text = playerName;
        ChangeReadyButtonColor(false);
    }

    public void ToggleReadyButton(bool value)
    {
        ready_btt.interactable = value;
    }

    public void ToggleTeamButton(bool value)
    {
        //changeTeam_btt.interactable = value;
    }

    public void ChangeName(string newName)
    {
        nickname_text.text = newName;
    }

    public void ChangeReadyButtonColor(bool isReady)
    {
        if (ready_btt == null) return;

        var colors = ready_btt.colors;
        var red = Color.red;
        var green = Color.green;

        if (isReady)
        {
            colors.normalColor = green;
            colors.selectedColor = green;
            colors.pressedColor = green;
            colors.disabledColor = green;
        }
        else
        {
            colors.selectedColor = red;
            colors.pressedColor = red;
            colors.normalColor = red;
            colors.disabledColor = red;
        }

        
        
        ready_btt.colors = colors;
    }

}
