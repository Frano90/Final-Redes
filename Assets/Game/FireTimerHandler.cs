using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTimerHandler : MonoBehaviour
{
    private Collider myCol;
    [SerializeField] private float timeBurning;
    [SerializeField] private float timeOff;
    private float _count;

    [SerializeField] private ParticleSystem fireEffect;
    void Awake()
    {
        myCol = GetComponent<Collider>();

        timeOff = Random.Range(2, 10);
    }

    void Update()
    {
        if (myCol.enabled)
        {
            _count += Time.deltaTime;

            if (_count >= timeBurning)
            {
                _count = 0;
                myCol.enabled = false;
                TurnOffFire();
            }
        }
        else
        {
            _count += Time.deltaTime;

            if (_count >= timeOff)
            {
                _count = 0;
                myCol.enabled = true;
                TurnOnFire();
                timeOff = Random.Range(2, 10);
            }
        }
        
        
    }

    void TurnOnFire()
    {
        if(!fireEffect.isPlaying)
            fireEffect.Play();
    }
    
    void TurnOffFire()
    {
        if(fireEffect.isPlaying)
            fireEffect.Stop();
    }
}
