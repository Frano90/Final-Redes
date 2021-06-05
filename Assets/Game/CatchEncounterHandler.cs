using System;
using System.Collections;
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

   private Vector3 leftBttPos, rightBttPos, centerBttPos;

   void Start()
   {

      if (photonView.IsMine) return;
      
      left_btt.onClick.AddListener(ChooseLeft);  
      center_btt.onClick.AddListener(ChooseCenter);  
      right_btt.onClick.AddListener(ChooseRight);

      leftBttPos = left_btt.transform.position;
      centerBttPos = center_btt.transform.position;
      rightBttPos = right_btt.transform.position;
   }

   void ResetButtonPositions()
   {
      left_btt.transform.position = leftBttPos;
      center_btt.transform.position = centerBttPos;
      right_btt.transform.position = rightBttPos;
   }

   void ChooseLeft()
   {
      SendRequest_ChosenPath(left);
      StartCoroutine(MovePortraitForward(left_btt));
   }
   
   void ChooseRight()
   {
      SendRequest_ChosenPath(right);
      StartCoroutine(MovePortraitForward(right_btt));
   }
   
   void ChooseCenter()
   {
      SendRequest_ChosenPath(center);
      StartCoroutine(MovePortraitForward(center_btt));
   }

   void SetButtons(bool value)
   {
      center_btt.interactable = right_btt.interactable = left_btt.interactable = value;
   }
   
   IEnumerator MovePortraitForward(Button selectedBtt)
   {
      float time = 0;
      do
      {
         time += Time.deltaTime;
         selectedBtt.transform.position += 12 * selectedBtt.transform.right * Time.deltaTime;

         yield return new WaitForEndOfFrame();
      } while (time < 3);

   }
   
   void SendRequest_ChosenPath(int path)
   {
      SetButtons(false);
      photonView.RPC("RPC_SavePathChosen", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer,path);
   }

   [PunRPC]
   void RPC_SavePathChosen(Player player, int path)
   {
      if (!_dicPlayerPath.ContainsKey(player))
      {
         _dicPlayerPath.Add(player, path);
      }
      
      RegisterPlayerChoice(player);

      if (cat == null || mouse == null) return;

      ResolveEncounter();
   }

   private void RegisterPlayerChoice(Player player)
   {
      var playerData = MyServer_FA.Instance.GetCharacterLobbyDataDictionary[player];
      var playerTeam = playerData.team;

      if (playerTeam == LobbySelectorData.Team.cat) cat = player;
      else mouse = player;
   }

   private void ResolveEncounter()
   {
      var catDir = _dicPlayerPath[cat];
      var mouseDir = _dicPlayerPath[mouse];
      
      bool catCatchMouse;

      if (catDir == center && mouseDir == center)
      {
         catCatchMouse = true;
      }else if (catDir == left && mouseDir == right || catDir == right && mouseDir == left)
      {
         catCatchMouse = true;
      }
      else
      {
         catCatchMouse = false;
      }

      StartCoroutine(WaitToExecutEventResult(catCatchMouse));

   }

   IEnumerator WaitToExecutEventResult(bool catCatchMouse)
   {
      yield return new WaitForSeconds(3);
      
      MyServer_FA.Instance.gameController.EncounterFeedbackResult(catCatchMouse, cat, mouse);
      photonView.RPC("RPC_SetPanel", cat, false);
      photonView.RPC("RPC_SetPanel", mouse, false);

      cat = mouse = null;
   }

   public void ActiveCatchEncounter(Player ratPlayer, Player catPlayer)
   {
      StopAllCoroutines();
      _dicPlayerPath.Clear();
      ;
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
      ResetButtonPositions();
      panel.SetActive(true);
      SetButtons(true);
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
