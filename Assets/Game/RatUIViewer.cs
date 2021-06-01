using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RatUIViewer : MonoBehaviour
{
    [SerializeField] private TMP_Text nickName;
    [SerializeField] private TMP_Text lifeRemain;
    [SerializeField] private Image portrait;
    
    public bool IsOcupied { get; private set; }
    public void SetPlayerUI(Sprite portrait, string nickname, string lifeCount)
    {
        this.portrait.sprite = portrait;
        nickName.text = nickname;
        lifeRemain.text = $"x {lifeCount}";

        IsOcupied = true;
    }

    public void SetOcupied()
    {
        IsOcupied = true;
    }

    public void RefreshLifeUI(int lifeCount)
    {
        lifeRemain.text = $"x {lifeCount}";
    }
}
