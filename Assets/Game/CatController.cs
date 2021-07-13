using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FranoW;

public class CatController : MonoBehaviour
{
    public CatCharacter_FA cat;
    float MouseVertSens = 5f;

    void Update()
    {   
        if (Input.GetKeyDown(KeyCode.Space) && cat.grounded)
        {
            cat.StartJump();
        }

        if (Input.GetKey(KeyCode.Space) && cat.isWaitingJump)
        {
            //cat.MoveCameraOnWait(Input.GetAxis("Mouse Y") * MouseVertSens * Time.deltaTime);
        }else
        {
            //cat.CamBackToPos();
        }

        if (Input.GetKeyUp(KeyCode.Space) && cat.isWaitingJump)
        {
            cat.ReleaseJump();
        }
    }
}
