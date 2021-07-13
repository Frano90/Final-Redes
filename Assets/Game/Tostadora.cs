using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Tostadora : MonoBehaviourPun
{
    [SerializeField] private Collider myCol;
    [SerializeField] private JumpPad_FA myjumpPad;
    private Animator _anim;

    [SerializeField] private Transform palanquita;

    private bool active;
    
    

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        myCol = GetComponent<Collider>();
    }

    [SerializeField] private LayerMask triggerLayers;
    private void OnTriggerEnter(Collider other)
    {
        if ((triggerLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            if(active) return;
            
            myCol.enabled = false;
            //_anim.Play("OffToster");
            active = true;
        }
    }

    private float _count;
    [SerializeField] private float timeToExpulse;
    private void Update()
    {
        if (!active) return;
        
        _count += Time.deltaTime;

        if (_count >= timeToExpulse)
        {
            active = false;
            //_anim.Play("OnToster");
            StartCoroutine(WaitToAnimation());
            _count = 0;
        }
        
        
        if (photonView.IsMine)
        {
            if(_anim != null) _anim.SetBool("triggered", active);
        }
    }

    [SerializeField] private float timeToWaitForReleaseAnimation;
    
    IEnumerator WaitToAnimation()
    {
        myjumpPad.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(timeToWaitForReleaseAnimation);

        yield return new WaitForSeconds(1f);

        myCol.enabled = true;
        myjumpPad.gameObject.SetActive(false);
    }
}
