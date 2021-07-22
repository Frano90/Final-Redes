using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FranoW;
using Photon.Pun;

public class CatCharacter_FA : Character_FA
{
    [Header("Vertical")]
    float MouseVertSens = 5f;
    
    public float jumpForce = 5;

    [Header("RatHunter")] 
    [SerializeField] private float checkRadius;

    [SerializeField] private LayerMask targetToHunt;
    
    public bool isWaitingJump;
    public float timer = 0;

    private void Start()
    {
        if (!photonView.IsMine) return;

        Cursor.lockState = CursorLockMode.Locked;
    }

    protected  override void Update()
    {
        if (!photonView.IsMine) return;
        
        base.Update();

        if (isWaitingJump)
        {
            timer += Time.deltaTime;
        }

        if(!inEncunter)
            CheckIfRatIsClose();
        
    }

    public void StartJump()
    {
        timer = 0;
        isWaitingJump = true;
    }

    public void Jump(Vector3 dir)
    {
        isWaitingJump = false;
        _impactRecivier.AddImpact(dir, 10 * jumpForce);
        photonView.RPC("RCP_DashFeedback", RpcTarget.Others);
    }

    Vector3 dir = Vector3.zero;
    public void ReleaseJump()
    {
        dir = Vector3.Lerp(transform.forward * 2 + Vector3.up, transform.forward + Vector3.up * jumpForce, timer / 2).normalized;

        Jump(dir);
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
