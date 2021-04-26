using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishPanel_UI : MonoBehaviour
{
    public Button continue_btt;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddEventToContinueButton(Action callback)
    {
        void InternalAction() {callback?.Invoke();}//que al pedo esto
        continue_btt.onClick.AddListener(InternalAction);
    }
}
