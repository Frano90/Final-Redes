using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static MyServer_FA;

public class Character_FA : MonoBehaviourPun
{
    public Player _owner{ get; private set;}
    
    UIController_FA uiController;

    public void Move(Vector3 dir, float speed)
    {
        transform.position += dir * speed * Time.deltaTime;
    }


    public Character_FA SetInitialParameters(Player localPlayer)
    {
        _owner = localPlayer;
        photonView.RPC("SetLocalParams", localPlayer);
        return this;
    }

    [PunRPC]
    void SetLocalParams()
    {
        uiController = FindObjectOfType<UIController_FA>();
    }
    
    
}
