using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FireTimerHandler : MonoBehaviourPun
{
    private Collider myCol;
    [SerializeField] private float timeBurning;
    [SerializeField] private float timeOff;
    private float _count;

    [SerializeField] private ParticleSystem fireEffect;
    void Awake()
    {
        if (!photonView.IsMine) return;

        myCol = GetComponent<Collider>();

        timeOff = Random.Range(2, 10);
    }

    void Update()
    {

        if (!photonView.IsMine) return;
        
        if (myCol.enabled)
        {
            _count += Time.deltaTime;

            if (_count >= timeBurning)
            {
                _count = 0;
                myCol.enabled = false;
                //TurnOffFire();
                photonView.RPC("RPC_TurnOffFire", RpcTarget.Others);
            }
        }
        else
        {
            _count += Time.deltaTime;

            if (_count >= timeOff)
            {
                _count = 0;
                myCol.enabled = true;
                photonView.RPC("RPC_TurnOnFire", RpcTarget.Others);
                //TurnOnFire();
                timeOff = Random.Range(2, 10);
            }
        }
        
        
    }

    [PunRPC]
    void RPC_TurnOnFire()
    {
        //if(!fireEffect.isPlaying)
            fireEffect.Play();
    }
    
    [PunRPC]
    void RPC_TurnOffFire()
    {
        //if(fireEffect.isPlaying)
            fireEffect.Stop();
    }
}
