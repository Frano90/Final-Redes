using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Spawners_FA : MonoBehaviourPun
{
    public Controller_FA currentController;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            MyServer_FA.Instance.RequestCreateModelPlayer(PhotonNetwork.LocalPlayer);
            currentController = PhotonNetwork.Instantiate(MyServer_FA.Instance.controller_pf.name, Vector3.zero, Quaternion.identity).GetComponent<Controller_FA>();
        }
    }

    public void RequestDeleteController()
    {
        Debug.Log("Entra al aserver");
        photonView.RPC("RPCDeleteController", RpcTarget.Others);
    }

    [PunRPC]
    public void RPCDeleteController()
    {
        Debug.Log("Entra al cliente");
        if (!photonView.IsMine && currentController != null)
        {
            PhotonNetwork.Destroy(currentController.gameObject);
            currentController = null;
        }
        
    }
}
