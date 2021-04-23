using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheeseScoreHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text score;
    public void RefreshLocalCheeseScore(int newValueScore, int maxCheese)
    {
        Debug.Log("llegue");
        score.text = $"{newValueScore} / {maxCheese}";
    }
}
