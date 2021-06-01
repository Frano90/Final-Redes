using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class CatchEncounterHandler : MonoBehaviourPun
{
   [SerializeField] private Image otherSideSprite;
   [SerializeField] private Image mySideSprite;

   [SerializeField] private Button left, center, right;

   [SerializeField] private GameObject panel;
   
   private void Start()
   {
      left.onClick.AddListener(ChooseLeft);  
      center.onClick.AddListener(ChooseCenter);  
      right.onClick.AddListener(ChooseRight);  
   }

   void ChooseLeft()
   {
      
   }
   
   void ChooseRight()
   {
      
   }
   
   void ChooseCenter()
   {
      
   }

   public void ActiveCatchEncounter(Player ratPlayer, Player catPlayer)
   {
      int ratDataIndex = GetIndexPortraitData(ratPlayer);
      int catDataIndex = GetIndexPortraitData(catPlayer);
      
      photonView.RPC("RPC_SetPortraits", ratPlayer, catDataIndex, ratDataIndex);
      photonView.RPC("RPC_SetPortraits", catPlayer, ratDataIndex, catDataIndex);
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
