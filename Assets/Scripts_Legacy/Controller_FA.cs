using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Controller_FA : MonoBehaviourPun
{
    [SerializeField] private float mouseSensitivity;
    private float xRotation;

    private bool activeInputs = true;
    
    private void Start()
    {
        
        if (!photonView.IsMine)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(SendPackage());
    }

    IEnumerator SendPackage()
    {
        while(true)
        {
            if(activeInputs)
                HandleInputs();
                        
            yield return new WaitForSeconds(1 / MyServer_FA.Instance.PackagePerSecond);
        }
    }

    protected virtual void HandleInputs()
    {
        HandleMovementInput();

        HandleRotationInput();
        
        HanldleStopMovement();
    }
    
    private void HanldleStopMovement()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MyServer_FA.Instance.RequestStopMovement(PhotonNetwork.LocalPlayer);
        }
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            MyServer_FA.Instance.RequestResumeMovement(PhotonNetwork.LocalPlayer);
        }
    }

    private void HandleMovementInput()
    {
        var h = Input.GetAxis("Horizontal");
        var v = 1f;

        if (h != 0 || v != 0)
        {
            Vector3 move = transform.right * h + transform.forward * v;
            MyServer_FA.Instance.RequestMove(PhotonNetwork.LocalPlayer, move.normalized);
        }
    }

    private void HandleRotationInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -20f, 15f);

        MyServer_FA.Instance.RequestRotation(PhotonNetwork.LocalPlayer, xRotation, mouseX);
    }
    
    public void DisableInputs()
    {
        activeInputs = false;
    }
    
    public void EnableInputs()
    {
        activeInputs = true;
    }
    
}
