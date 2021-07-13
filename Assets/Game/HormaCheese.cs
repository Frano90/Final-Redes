using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class HormaCheese : MonoBehaviourPun
{

    [SerializeField] private Transform cheeseSpawn_pos;

    private GameObject cheese;
    private void Start()
    {
        if(!photonView.IsMine) return;
        
        SpawnCheese();
    }


    public float _count = 0; 
    
    private void Update()
    {
        if(!photonView.IsMine) return;

        if (cheese != null) return;
            
        _count += Time.deltaTime;
        
        if(_count >= 3.5f)
            SpawnCheese();
    }

    void SpawnCheese()
    {
        Debug.Log("spawn cheese");
        _count = 0;
        cheese = PhotonNetwork.Instantiate("Cheese", cheeseSpawn_pos.position, Quaternion.identity);
    }
}
