using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Controller_FA : MonoBehaviourPun
{
    [SerializeField] private float mouseSensitivity;
    private float xRotation;

    private void Start()
    {
        
        if (!photonView.IsMine)
            return;

        StartCoroutine(SendPackage());
    }

    IEnumerator SendPackage()
    {
        
        while(true)
        {
            // if (Input.GetKeyDown(KeyCode.O))
            // {
            //     //MyServer_FA.Instance.RequestWin(PhotonNetwork.LocalPlayer);
            // }
            
            HandleMovementInput();

            HandleRotationInput();

            HandleDashInput();

            HanldleStopMovement();
            
            yield return new WaitForSeconds(1 / MyServer_FA.Instance.PackagePerSecond);
        }
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

    private void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            MyServer_FA.Instance.RequestDash(PhotonNetwork.LocalPlayer);
        }
    }

    private void HandleMovementInput()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        //Lo deje para probar y que me quede el ejemplo


        if (h != 0 || v != 0)
        {
            Vector3 move = transform.right * h + transform.forward * v;
            MyServer_FA.Instance.RequestMove(PhotonNetwork.LocalPlayer, move.normalized);
        }
        else
        {
            MyServer_FA.Instance.RequestMove(PhotonNetwork.LocalPlayer, Vector3.zero);
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
}
