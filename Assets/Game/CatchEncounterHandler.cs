using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class CatchEncounterHandler : MonoBehaviourPun
{
   private const int left = -1; 
   private const int center = 0; 
   private const int right = 1; 
   
   [SerializeField] private Image otherSideSprite;
   [SerializeField] private Image mySideSprite;

   [SerializeField] private Button left_btt, center_btt, right_btt;

   [SerializeField] private GameObject panel;

   private Player cat, mouse;

   private Dictionary<Player, int> _dicPlayerPath = new Dictionary<Player, int>();
   
   //public Dictionary<Player, ChosenPath> GetDicPlayerPath

   void Start()
   {
      if (photonView.IsMine) return;
      
      left_btt.onClick.AddListener(ChooseLeft);  
      center_btt.onClick.AddListener(ChooseCenter);  
      right_btt.onClick.AddListener(ChooseRight);  
   }

   void ChooseLeft()
   {
      SendRequest_ChosenPath(left);
   }
   
   void ChooseRight()
   {
      SendRequest_ChosenPath(right);
   }
   
   void ChooseCenter()
   {
      SendRequest_ChosenPath(center);
   }
   
   void SendRequest_ChosenPath(int path)
   {
      photonView.RPC("RPC_SavePathChosen", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer,path);
   }

   [PunRPC]
   void RPC_SavePathChosen(Player player, int path)
   {
      if (!_dicPlayerPath.ContainsKey(player))
      {
         Debug.Log("agrego a la k " + player.NickName + " y value " + path);
         _dicPlayerPath.Add(player, path);
      }
      
      var playerData = MyServer_FA.Instance.GetCharacterLobbyDataDictionary[player];
      Debug.Log(playerData.team);
      var playerTeam = playerData.team;

      Debug.Log("el personaje " + playerTeam + " eligio " + path);
      
      
      //Ver que el error esta aca creo
      if (playerTeam == LobbySelectorData.Team.cat) cat = player;
      else mouse = player;
      
      
      
      if (cat == null || mouse == null) return;

       Debug.Log(cat);
       Debug.Log(mouse);
      
      ResolveEncounter();
   }

   private void ResolveEncounter()
   {
      
      Debug.Log(cat);
      Debug.Log(mouse);
      var catDir = _dicPlayerPath[cat];
      var mouseDir = _dicPlayerPath[mouse];
      
      bool catCatchMouse;
      //TODO estan dando 0 los dos aunque no sea lo elegido. De parte del local esta bien
      Debug.Log("la dir de gato es " + catDir);
      Debug.Log("la dir de raton es " + mouseDir);
      
      if (catDir == center && mouseDir == center)
      {
         Debug.Log("entra en center");
         catCatchMouse = true;
      }else if (catDir == left && mouseDir == right || catDir == right && mouseDir == left)
      {
         Debug.Log("entra en misma no centro");
         catCatchMouse = true;
      }
      else
      {
         Debug.Log("entra en resto");
         catCatchMouse = false;
      }

      MyServer_FA.Instance.gameController.EncounterFeedbackResult(catCatchMouse, cat, mouse);
      photonView.RPC("RPC_SetPanel", cat, false);
      photonView.RPC("RPC_SetPanel", mouse, false);

      cat = mouse = null;

   }

   public void ActiveCatchEncounter(Player ratPlayer, Player catPlayer)
   {
      _dicPlayerPath.Clear();
      int ratDataIndex = GetIndexPortraitData(ratPlayer);
      int catDataIndex = GetIndexPortraitData(catPlayer);
      
      photonView.RPC("RPC_SetPortraits", ratPlayer, catDataIndex, ratDataIndex);
      photonView.RPC("RPC_SetPortraits", catPlayer, ratDataIndex, catDataIndex);
   }

   [PunRPC]
   void RPC_SetPanel(bool on)
   {
      panel.SetActive(on);
   }

   [PunRPC]
   void RPC_SetPortraits(int otherPlayerIndexData, int myDataIndex)
   {
      panel.SetActive(true);
      
      Sprite otherPortrait = MyServer_FA.Instance.lobySelectorDatas[otherPlayerIndexData].portrait;
      Sprite myPortrait = MyServer_FA.Instance.lobySelectorDatas[myDataIndex].portrait;

      otherSideSprite.sprite = otherPortrait;
      mySideSprite.sprite = myPortrait;
   }

   int GetIndexPortraitData(Player player)
   {
      var desiredPortrait = MyServer_FA.Instance.GetCharacterLobbyDataDictionary[player].portrait;
      var index = -1;
      for (int i = 0; i < MyServer_FA.Instance.lobySelectorDatas.Count; i++)
      {
         if (MyServer_FA.Instance.lobySelectorDatas[i].portrait.Equals(desiredPortrait))
         {
            index = i;
            break;
         }
      }

      return index;
   }
}
