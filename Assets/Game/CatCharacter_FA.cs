using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FranoW;
using Photon.Pun;

public class CatCharacter_FA : Character_FA
{
    [Header("General")]    
    public CameraFollow camFollow;

    [Header("Horizontal")]
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;

    [Header("Vertical")]
    float MouseVertSens = 5f;

    [Header("Jump")]
    [SerializeField] float jumpHeight = 3f;
    public float jumpForce = 5;

    [Header("RatHunter")] 
    [SerializeField] private float checkRadius;

    [SerializeField] private LayerMask targetToHunt;
    
    public bool isWaitingJump;
    public float timer = 0;

    float gravity = -9.81f;
    [SerializeField] float gravityScaler = 1;
    public float Gravedad => gravity * gravityScaler;

    public Vector3 velocity;

    private void Start()
    {
        if (!photonView.IsMine) return;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (isWaitingJump)
        {
            timer += Time.deltaTime;
        }
        else
        {
            Move();
        }

        if(!inEncunter)
            CheckIfRatIsClose();
        
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        camFollow.ChangeCamPos(timer / 2);
    }

    // public override void SetModelRender(bool value)
    // {
    //     photonView.RPC("RPC_SetModelRender", _owner, value);
    // }
    //
    // [PunRPC]
    // void RPC_SetModelRender(bool value)
    // {
    //     myMeshRenderer.enabled = value;
    // }
    

    public void MoveCameraOnWait(float val)
    {
        timer = Mathf.Clamp(timer + val, 0, 2);
    }

    public void Move()
    {

        if (movementLocked) return;
        
        Vector3 dir = new Vector3(0, 0, 1);

        if (dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + myCam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            controller.Move(transform.forward * speed * Time.deltaTime);
        }

        float horizontalCam = Input.GetAxis("Mouse X");

        if(grounded) transform.Rotate(transform.up, horizontalCam);

        if (!grounded) velocity = Gravity.Apply(velocity, gravity, gravityScaler, controller);
        else velocity = new Vector3(velocity.x, 0, velocity.z);
        
    }

    public void StartJump()
    {
        if (!grounded) return;
        timer = 0;
        isWaitingJump = true;
        camInPlace = false;
    }

    public void Jump(Vector3 dir)
    {
        isWaitingJump = false;
        _impactRecivier.AddImpact(dir, 10 * jumpForce);
    }

    Vector3 dir = Vector3.zero;
    public void ReleaseJump()
    {
        dir = Vector3.Lerp(transform.forward * 2 + Vector3.up, transform.forward + Vector3.up * jumpForce, timer / 2).normalized;

        Jump(dir);
    }

    bool camInPlace;

    public void CamBackToPos()
    {
        if (timer > 0 && !camInPlace)
        {
            timer -= 3 * Time.deltaTime;

            if (timer <= 0) camInPlace = true;
        }
    }

    void CheckIfRatIsClose()
    {
        var ratsClose = Physics.OverlapSphere(transform.position, checkRadius, targetToHunt);

        if (ratsClose.Length > 0)
        {
            MyServer_FA.Instance.gameController.StartCatchEncounter(ratsClose[0].GetComponent<RatCharacter_FA>()._owner, _owner);
        }
    }
    
    [PunRPC]
    protected override void RPC_SetModelRender(bool value)
    {
        photonView.RPC("RPC_SetRender", RpcTarget.Others, value);
        
    }

    [PunRPC]
    protected override void RPC_SetRender(bool value)
    {
        myMeshRenderer.enabled = value;
    }
    

}
