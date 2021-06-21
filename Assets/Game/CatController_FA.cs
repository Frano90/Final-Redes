using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CatController_FA : Controller_FA
{
    float MouseVertSens = 5f;
    
    protected override void HandleInputs()
    {
        base.HandleInputs();
        
        HandleJumpInput();
    }

    void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))// && cat.grounded)
        {
            MyServer_FA.Instance.RequestJump(PhotonNetwork.LocalPlayer);
        }

        if (Input.GetKeyUp(KeyCode.Space))// && cat.isWaitingJump)
        {
            MyServer_FA.Instance.RequestReleaseJump(PhotonNetwork.LocalPlayer);
        }
    }
}
