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
            //cat.StartJump();
        }
        
        if (Input.GetKey(KeyCode.Space))// && cat.isWaitingJump)
        {
            var value = Input.GetAxis("Mouse Y") * MouseVertSens * Time.deltaTime;
            MyServer_FA.Instance.RequestMoveCameraOnWait(PhotonNetwork.LocalPlayer, value);
            //cat.MoveCameraOnWait(Input.GetAxis("Mouse Y") * MouseVertSens * Time.deltaTime);
        }else
        {
            MyServer_FA.Instance.RequestCamBackToPos(PhotonNetwork.LocalPlayer);
            //cat.CamBackToPos();
        }
        
        if (Input.GetKeyUp(KeyCode.Space))// && cat.isWaitingJump)
        {
            MyServer_FA.Instance.RequestReleaseJump(PhotonNetwork.LocalPlayer);
            //cat.ReleaseJump();
        }
    }
}
