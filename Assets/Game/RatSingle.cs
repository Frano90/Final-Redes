using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FranoW;

public class RatSingle : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity;
    private float xRotation;

    [SerializeField] protected CharacterController controller;
    protected ImpactReceiver _impactRecivier;
    public float speed;
    [SerializeField] protected Camera myCam;

    [SerializeField] protected Transform groundCheck;

    private bool _imDashing;
    [SerializeField] protected bool movementLocked = false;
    public bool grounded;
    [SerializeField] protected float groundDistance;
    [SerializeField] protected LayerMask groundMask;

    private Vector3 startPosition;

    public bool inEncunter { get; private set; }

    public bool IsMovementLocked => movementLocked;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        _impactRecivier = GetComponent<ImpactReceiver>();
    }
    private void Update()
    {
        HandleMovementInput();
        HandleRotationInput();
        HandleDashInput();
        HanldleStopMovement();
        
        if (grounded) jumptimer = 0;
        else
            jumptimer += Time.deltaTime;

        
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }
    }

    private void HandleMovementInput()
    {
        var h = Input.GetAxis("Horizontal");
        var v = 1f;       

        if (h != 0 || v != 0)
        {
            Vector3 dir = transform.forward * v;
            Move(dir.normalized, speed);
        }
    }
    private void HandleRotationInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -20f, 15f);

        Rotate(xRotation, mouseX);
    }

    private void HanldleStopMovement()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopMovement();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            ResumeMovement();
        }
    }
    
    public virtual void Move(Vector3 dir, float speed)
    {
        if (movementLocked) return;
        controller.Move(dir * speed * Time.deltaTime);
    }

    public void SetEncounter(bool inCombat)
    {
        inEncunter = inCombat;
    }

    float jumptimer = 0;

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

    public void Dash()
    {
        if (_imDashing || movementLocked) return;

        _imDashing = true;
        _impactRecivier.AddImpact(transform.forward, 20);
        _impactRecivier.AddImpact(transform.up, 10);

        Invoke("ResetDashCD", 2f);
    }

    void ResetDashCD() => _imDashing = false;

    // public void ApplyGravity()
    // {
    //     Vector3 vel = Vector3.zero;
    //
    //     grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    //
    //     if (jumptimer > 1f)
    //     {
    //         vel.y = -10f;
    //     }
    //     else
    //         vel.y = -10 * jumptimer;
    //
    //     Gravity.ApplyDefault(vel, controller);
    // }
    
    public void ApplyGravity()
    {
        Vector3 vel = Vector3.zero;

        CheckGrounded();

        if (grounded && vel.y < 0)
        {
            vel.y = -2f;
        }
        
        Gravity.ApplyDefault(vel, controller);
    }
    
    private float auxCount;
    private void CheckGrounded()
    {
        if (Physics.CheckSphere(groundCheck.position, groundDistance, groundMask))
        {
            auxCount = 0;
            grounded = true;
        }
        else
        {
            auxCount += Time.deltaTime;

            if (auxCount >= 0)
            {
                if(grounded)
                    _impactRecivier.AddImpact((Vector3.up + transform.forward).normalized, 20);
                
                grounded = false;
            }
        }
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
