using System;
using UnityEngine;

public class PlayerInput_Controller
{
    KeyCode mouse0;
    KeyCode mouse1;


    public event Action OnPressDownMouse0;
    public event Action OnPressDownMouse1;

    public event Action OnPressUpMouse0;
    public event Action OnPressUpMouse1;

    public PlayerInput_Controller ConfigureKeys(KeyCode mouse0, KeyCode mouse1)
    {
        this.mouse0 = mouse0;
        this.mouse1 = mouse1;
        return this;
    }

    public PlayerInput_Controller ConfigureCallbacks(Action mouse0Down, Action mouse1Down, Action mouse0Up, Action mouse1Up)
    {
        OnPressDownMouse0 = mouse0Down;
        OnPressDownMouse1 = mouse1Down;
        OnPressUpMouse0 = mouse0Up;
        OnPressUpMouse1 = mouse1Up;
        return this;
    }


    public void OnUpdate()
    {       
        if (Input.GetKeyDown(mouse0)) OnPressDownMouse0?.Invoke();
        else if (Input.GetKeyDown(mouse1)) OnPressDownMouse1?.Invoke();
        else if (Input.GetKeyUp(mouse0)) OnPressUpMouse0?.Invoke();
        else if (Input.GetKeyUp(mouse1)) OnPressUpMouse1?.Invoke();
    }
}

