using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Spawners_FA : MonoBehaviourPun
{
    public RatController_FA ratController_pf;
    public CatController_FA catController_pf;

    Controller_FA currentController;

    [SerializeField] private Transform catStartingPosition;
    [SerializeField] private Transform ratStartingPosition;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            photonView.RPC("RPC_RequestCreateModelPlayer", MyServer_FA.Instance.GetServer, PhotonNetwork.LocalPlayer);
        }
    }


    [PunRPC]
    private void RPC_RequestCreateModelPlayer(Player localPlayer)
    {
        var playerData = MyServer_FA.Instance.GetCharacterLobbyDataDictionary[localPlayer];

        Character_FA newModel;
        
        if (playerData.team == LobbySelectorData.Team.cat)
        {
            newModel = PhotonNetwork.Instantiate("CatChar", catStartingPosition.position, Quaternion.identity).GetComponent<CatCharacter_FA>().SetInitialParameters(localPlayer);
            photonView.RPC("CreateController", localPlayer, "cat");
        }
        else
        {
            newModel = PhotonNetwork.Instantiate("RatTest", ratStartingPosition.position, Quaternion.identity).GetComponent<RatCharacter_FA>().SetInitialParameters(localPlayer);
            photonView.RPC("CreateController", localPlayer, "rat");
        }

        MyServer_FA.Instance.GetModels[localPlayer] = newModel;
        MyServer_FA.Instance.gameController.AddModel(localPlayer, MyServer_FA.Instance.GetModels[localPlayer]);
        MyServer_FA.Instance.UI_controller.RegisterPlayerUI(localPlayer);
    }

    [PunRPC]

    void CreateController(string controllerType)
    {
        if (controllerType == "cat")
        {
            currentController = PhotonNetwork.Instantiate(catController_pf.name, Vector3.zero, Quaternion.identity).GetComponent<CatController_FA>();
        }
        else if(controllerType == "rat")
        {
            currentController = PhotonNetwork.Instantiate(ratController_pf.name, Vector3.zero, Quaternion.identity).GetComponent<CatController_FA>();
        }
        else
        {
            Debug.LogError("Se dio mal el tipo de controlador");
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
