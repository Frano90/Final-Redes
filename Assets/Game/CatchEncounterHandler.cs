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

   private Vector3 mySide_pos, oppositeSide_pos;

   [SerializeField] private Text feedbackText;

   void Start()
   {

      if (photonView.IsMine) return;
      
      left_btt.onClick.AddListener(ChooseLeft);  
      center_btt.onClick.AddListener(ChooseCenter);  
      right_btt.onClick.AddListener(ChooseRight);

      mySide_pos = mySideSprite.transform.position;
      oppositeSide_pos = otherSideSprite.transform.position;
   }

   void ResetButtonPositions()
   {
      otherSideSprite.transform.position = oppositeSide_pos;
      mySideSprite.transform.position = mySide_pos;
   }

   void ChooseLeft()
   {
      SendRequest_ChosenPath(left);
      StartCoroutine(MoveMyPortraitToSelectedPosition(mySideSprite.transform, left));
   }
   
   void ChooseRight()
   {
      SendRequest_ChosenPath(right);
      StartCoroutine(MoveMyPortraitToSelectedPosition(mySideSprite.transform, right));
   }
   
   void ChooseCenter()
   {
      SendRequest_ChosenPath(center);
      StartCoroutine(MoveMyPortraitToSelectedPosition(mySideSprite.transform, center));
   }

   void SetButtons(bool value)
   {
      center_btt.interactable = right_btt.interactable = left_btt.interactable = value;
      center_btt.gameObject.SetActive(value);
      left_btt.gameObject.SetActive(value);
      right_btt.gameObject.SetActive(value);
   }

   IEnumerator MoveOppositePortraitToSelectedPosition(Transform objectToMove ,int chosenPos)
   {
      float time = 0;                                      
      Vector3 dirToMove = ((-objectToMove.up) + chosenPos * objectToMove.right).normalized;  
      
      do
      {
         time += Time.deltaTime;
         objectToMove.position += 45 * dirToMove * Time.deltaTime;

         yield return new WaitForEndOfFrame();
      } while (time < 3);
      
   }
   
   IEnumerator MoveMyPortraitToSelectedPosition(Transform objectToMove ,int chosenPos)
   {
      float time = 0;                                      
      Vector3 dirToMove = ((objectToMove.up) + chosenPos * objectToMove.right).normalized;  
      
      do
      {
         time += Time.deltaTime;
         objectToMove.position += 45 * dirToMove * Time.deltaTime;

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

      photonView.RPC("RPC_MoveOppositePortrait", cat, mouseDir, catCatchMouse);
      photonView.RPC("RPC_MoveOppositePortrait", mouse, catDir, catCatchMouse);
      StartCoroutine(WaitToExecutEventResult(catCatchMouse));

   }

   [PunRPC]
   void RPC_MoveOppositePortrait(int chosenPos, bool catCatchesMouse)
   {
      StartCoroutine(MoveOppositePortraitToSelectedPosition(otherSideSprite.transform, (-chosenPos)));
      
      feedbackText.text = catCatchesMouse ? "ATRAPADO" : "ESCAPO";
   }

   IEnumerator WaitToExecutEventResult(bool catCatchMouse)
   {
      
      
      
      yield return new WaitForSeconds(6);
      
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
      feedbackText.text = "";
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
