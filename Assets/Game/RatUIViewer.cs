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
    void SetPlayerUI(Sprite portrait, string nickname, string lifeCount)
    {
        this.portrait.sprite = portrait;
        nickName.text = nickname;
        lifeRemain.text = $"x {lifeCount}";
    }

    public void GainLife()
    {
        
    }

    public void LoseLife()
    {
        
    }
}
