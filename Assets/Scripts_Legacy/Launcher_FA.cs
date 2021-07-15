using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Launcher_FA : MonoBehaviourPunCallbacks
{
    public MyServer_FA server_pf;
    
    public void BtnConnect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 5;
        PhotonNetwork.JoinOrCreateRoom("Fran", options, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
      PhotonNetwork.Instantiate(server_pf.name, Vector3.zero, Quaternion.identity).GetComponent<MyServer_FA>();
    }

    public override void OnJoinedRoom()
    {

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("pasa esto en player  launch");
            StartCoroutine(WaitUntilServerCreated());
        }
        else
        {
            Debug.Log("pasa esto en server  launch");
        }
    }

    IEnumerator WaitUntilServerCreated()
    {
        while(MyServer_FA.Instance == null)
        {
            yield return new WaitForEndOfFrame();
        }

        MyServer_FA.Instance.RequestEnterLobby(PhotonNetwork.LocalPlayer);
        

    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Connection failed: " + cause.ToString());
    }

}
