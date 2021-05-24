using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RatController_FA : Controller_FA
{
    protected override void HandleInputs()
    {
        base.HandleInputs();
        
        HandleDashInput();
    }

    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            MyServer_FA.Instance.RequestDash(PhotonNetwork.LocalPlayer);
        }
    }
}
