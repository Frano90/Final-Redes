using System;
using System.Collections;
using System.Collections.Generic;
using FranoW;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static MyServer_FA;

public class Character_FA : MonoBehaviourPun
{
    public Player _owner{ get; private set;}
    
    UIController_FA uiController;
    [SerializeField] protected CharacterController controller;
    protected ImpactReceiver _impactRecivier;
    public float speed;
    [SerializeField] protected Camera myCam;

    [SerializeField] protected Transform groundCheck;
    
    private bool _imDashing;
    bool movementLocked = false;
    public bool grounded;
    [SerializeField] protected float groundDistance;
    [SerializeField] protected LayerMask groundMask;

    private Vector3 startPosition;

    public void Move(Vector3 dir, float speed)
    {
        if (movementLocked) return;
        
        dir = (transform.forward * dir.z).normalized;
        controller.Move(dir * speed * Time.deltaTime);
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        
        ApplyGravity();
    }

    public void Rotate(float xRotation, float mouseX)
    {
        myCam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    
    
    public void ResetCharacter()
    {
        controller.enabled = false;
        transform.position = startPosition;
        controller.enabled = true;
    }
    
    public virtual Character_FA SetInitialParameters(Player localPlayer, Vector3 startingPos)
    {
        _owner = localPlayer;
        controller = GetComponent<CharacterController>();
        _impactRecivier = GetComponent<ImpactReceiver>();

        startPosition = startingPos;
        
        photonView.RPC("SetLocalParams", localPlayer);
        return this;
    }
    
    
    [PunRPC]
    protected void SetLocalParams()
    {
        myCam.gameObject.SetActive(true);
        uiController = FindObjectOfType<UIController_FA>();
    }

    public void Dash()
    {
        if(_imDashing || movementLocked) return;

        _imDashing = true;
        _impactRecivier.AddImpact(transform.forward, 50);
        
        Invoke("ResetDashCD", 2f);
    }

    void ResetDashCD() => _imDashing = false;
    
    public void ApplyGravity()
    {
        Vector3 vel = Vector3.zero;

        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (grounded && vel.y < 0)
        {
            vel.y = -2f;
        }
        
        Gravity.ApplyDefault(vel, controller);
    }

    public void StopMovement()
    {
        movementLocked = true;
    }

    public void ResumeMovement()
    {
        movementLocked = false;
    }
}
