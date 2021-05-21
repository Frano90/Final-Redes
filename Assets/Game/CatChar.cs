using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FranoW;

public class CatChar : MonoBehaviour
{
    [Header("General")]
    public CharacterController controller;
    public Transform myCam;
    public float speed;
    public CameraFollow camFollow;
    public ImpactReceiver impactReceiver;


    [Header("Horizontal")]
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;

    [Header("Vertical")]
    float MouseVertSens = 5f;

    [Header("Jump")]
    [SerializeField] float jumpHeight = 3f;
    public float jumpForce = 5;
    [SerializeField] Transform groundCheck = null;
    [SerializeField] float groundDistance = .4f;
    [SerializeField] LayerMask groundMask;
    public bool isGrounded = false;
    public bool isWaitingJump;
    float timer = 0;
   
    float gravity = -9.81f;
    [SerializeField] float gravityScaler = 1;
    public float Gravedad => gravity * gravityScaler;        

    public Vector3 velocity;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        impactReceiver = GetComponent<ImpactReceiver>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            StartJump();
        }

        if (Input.GetKey(KeyCode.Space) && isWaitingJump)
        {
            MoveCameraOnWait(Input.GetAxis("Mouse Y") * MouseVertSens * Time.deltaTime);
        }
        else
        {
            CamBackToPos();
        }

        if (Input.GetKeyUp(KeyCode.Space) && isWaitingJump)
        {
            ReleaseJump();
        }

        if (!isWaitingJump)
            Move();              
      
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        camFollow.ChangeCamPos(timer / 2);
    }

    public void MoveCameraOnWait(float val)
    {
        timer = Mathf.Clamp(timer + val, 0, 2);
    }

    public void Move()
    {
        //float horizontal = Input.GetAxis("Horizontal");

        Vector3 dir = new Vector3(0, 0, 1);

        if (dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + myCam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            controller.Move(transform.forward * speed * Time.deltaTime);
        }

        float horizontalCam = Input.GetAxis("Mouse X");
        
        if(isGrounded)
            transform.Rotate(transform.up, horizontalCam);

        if (!isGrounded) velocity = Gravity.Apply(velocity, gravity, gravityScaler, controller);
        else velocity = new Vector3(velocity.x, 0, velocity.z);


        //if (!isJumping) controller.Move(new Vector3(0, -0.1f, 0));
    }

    public void StartJump()
    {
        if (!isGrounded) return;
        timer = 0;
        isWaitingJump = true;
        camInPlace = false;
    }

    public void Jump(Vector3 dir)
    {
        isWaitingJump = false;
        impactReceiver.AddImpact(dir, 20);
    }

    public void ReleaseJump()
    {
        Vector3 dir = Vector3.Lerp(transform.forward * 2 + Vector3.up, transform.forward + Vector3.up * jumpForce, timer / 2).normalized;

        Jump(dir);
    }

    bool camInPlace;

    public void CamBackToPos()
    {
        if (timer > 0 && !camInPlace)
        {
            timer -= 3*Time.deltaTime;

            if (timer <= 0) camInPlace = true;
        }
    }
}
