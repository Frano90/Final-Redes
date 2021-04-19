using System.Collections;
using System.Collections.Generic;
using FranoW;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController controller;

    [SerializeField] float speed = 0;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] float turnSpeed = 3f;

    [SerializeField] float fall_Scaler, lowJump_Scaler, gravityScaler = 0;

    [SerializeField] Transform groundCheck = null;
    [SerializeField] float groundDistance = .4f;
    [SerializeField] LayerMask groundMask;

    public float Gravedad => gravity * gravityScaler;

    

    public bool grounded;
    bool imDashing;
    bool rotating;

    public bool movementLocked = false;



    public Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void LockMovement()
    {
        movementLocked = true;
    }

    void ReleaseMovement()
    {
        movementLocked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            movementLocked = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            movementLocked = false;
        }

        if (movementLocked) return;


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }

        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        

        float x = 0f;//Input.GetAxis("Horizontal");
        float z = 1f;//Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);


        

        if(Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        CustomJumpFeel();
        
        velocity = Gravity.Apply(velocity, gravity, gravityScaler, controller);
    }
    

    void CustomJumpFeel()
    {
        if (velocity.y < -2f)
        {
            gravityScaler = fall_Scaler;
        }
        else if (velocity.y > 0 && !Input.GetButton("Jump"))
        {
            gravityScaler = lowJump_Scaler;
        }
        else
        {
            gravityScaler = 1f;
        }
    }

    void Dash()
    {       
        if (imDashing) return;

        imDashing = true;

        GetComponent<ImpactReceiver>().AddImpact(transform.forward, 50);

        Invoke("ResetDashCD", 1f);
    }

    void Rotate(int side)
    {
        rotating = true;

        if(side == -1)
        {
            transform.forward = Vector3.Lerp(transform.forward, -transform.right, turnSpeed * Time.deltaTime);
        }else if( side == 1)
        {
            transform.forward = Vector3.Lerp(transform.forward, transform.right, turnSpeed * Time.deltaTime);
        }
    }

    void ResetDashCD() => imDashing = false;
}
