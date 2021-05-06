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
    
    [SerializeField] Button changeTeam_btt;
    [SerializeField] Button ready_btt;
    [SerializeField] Text nickname_text;
    [SerializeField] Image portrait_img;

    public Action<int> onPressed_ChangeTeam_btt;
    public Action<int> onPressed_Ready_btt;


    public LobbySelectorData currentData { get; private set; }
    
    public int playerIndex;

    public bool imReady = false;

    public CharSelect_FA SetInitialParams(int index)
    {
        playerIndex = index;
        Debug.Log("suscribo eventos en selector");
        changeTeam_btt.onClick.AddListener(() => OnPressed_changeTeam(index));
        ready_btt.onClick.AddListener(() => OnPressed_ready(index));


        currentData = MyServer_FA.Instance.lobySelectorDatas[playerIndex];
        
        return this;
    }

    void OnPressed_changeTeam(int index) {onPressed_ChangeTeam_btt?.Invoke(index);}
    void OnPressed_ready(int index) => onPressed_Ready_btt?.Invoke(index);

    public void SetInitialView(string playerName, int indexPlayerData)
    {
        nickname_text.text = playerName;
        portrait_img.sprite = MyServer_FA.Instance.lobySelectorDatas[indexPlayerData].portrait;
        ChangeReadyButtonColor(false);
    }

    public void UpdateView(int indexPlayerData, string playerName, bool isReady)
    {
        nickname_text.text = playerName;
        portrait_img.sprite = MyServer_FA.Instance.lobySelectorDatas[indexPlayerData].portrait;
        ChangeReadyButtonColor(isReady);
    }

    public void ToggleReadyButton(bool value)
    {
        ready_btt.interactable = value;
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

    public void ChangeCharacter()
    {
        //int currentIndex = 0;
        var datas = MyServer_FA.Instance.lobySelectorDatas;
        for (int i = 0; i < datas.Count; i++)
        {
            if (datas[i].Equals(currentData))
            {
                if (i + 1 >= datas.Count)
                {
                    currentData = datas[0];
                    //currentIndex = 0;
                }
                else
                {
                    currentData = datas[i + 1];
                    //currentIndex = i + 1;
                }
                //MyServer_FA.Instance.RefreshPlayerLobbyData(currentIndex);
                break;
            }
        }
        // Debug.Log(portrait_img + "portrait");
        // Debug.Log(currentData + "data");
        
        portrait_img.sprite = currentData.portrait;
    }

    public void ToggleTeamButton(bool b)
    {
        changeTeam_btt.interactable = b;
    }
}
