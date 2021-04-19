using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static MyServer_FA;

public class Character_FA : MonoBehaviourPun
{
    public Player _owner{ get; private set;}
    
    UIController_FA uiController;
    [SerializeField] private CharacterController controller;
    public float speed;
    [SerializeField] private Camera myCam;

    public void Move(Vector3 dir, float speed)
    {
        dir = (transform.forward * dir.z).normalized;
        controller.Move(dir * speed * Time.deltaTime);
    }


    public void Rotate(float xRotation, float mouseX)
    {
        myCam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void ResetCharacter(Vector3 initialPos)
    {
        controller.enabled = false;
        transform.position = initialPos;
        controller.enabled = true;
    }
    
    public Character_FA SetInitialParameters(Player localPlayer)
    {
        _owner = localPlayer;
        controller = GetComponent<CharacterController>();
        photonView.RPC("SetLocalParams", localPlayer);
        return this;
    }

    [PunRPC]
    void SetLocalParams()
    {
        uiController = FindObjectOfType<UIController_FA>();
    }
}
