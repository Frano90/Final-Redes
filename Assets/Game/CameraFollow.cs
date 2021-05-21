using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform myCamera;
    public Transform[] lookAt;

    public Transform[] pos;

    public Vector3 offSet;

    public float posLerp = 0;

    void Update()
    {
        myCamera.LookAt(Vector3.Lerp(lookAt[0].position, lookAt[1].position, posLerp));
        
        myCamera.transform.position = Vector3.Lerp(pos[0].position, pos[1].position, posLerp);

    }

    public void ChangeCamPos(float lerpVal)
    {
        posLerp = lerpVal;
    }
}
