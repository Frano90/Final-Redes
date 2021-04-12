using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Controller_FA : MonoBehaviourPun
{
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
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");

            //Lo deje para probar y que me quede el ejemplo
            
            if( h != 0 || v != 0)
            {
                var dir = new Vector3(h, v, 0);
                MyServer_FA.Instance.RequestMove(PhotonNetwork.LocalPlayer, dir.normalized);
            }
            else
            {
                MyServer_FA.Instance.RequestMove(PhotonNetwork.LocalPlayer, Vector3.zero);
            }
            
            yield return new WaitForSeconds(1 / MyServer_FA.Instance.PackagePerSecond);
        }
    }
}
