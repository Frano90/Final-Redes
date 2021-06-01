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

   public void ActiveCatchEncounter(Player ratPlayer, Player catplayer)
   {
      photonView.RPC("RPC_SetPortraits", ratPlayer, catplayer);
      photonView.RPC("RPC_SetPortraits", catplayer, ratPlayer);
   }

   [PunRPC]
   void RPC_SetPortraits(Player otherPlayer)
   {
      panel.SetActive(true);
      
      Sprite otherPortrait = MyServer_FA.Instance.GetCharacterLobbyDataDictionary[otherPlayer].portrait;
      Sprite myPortrait = MyServer_FA.Instance.GetCharacterLobbyDataDictionary[PhotonNetwork.LocalPlayer].portrait;

      otherSideSprite.sprite = otherPortrait;
      mySideSprite.sprite = myPortrait;
   }
}
