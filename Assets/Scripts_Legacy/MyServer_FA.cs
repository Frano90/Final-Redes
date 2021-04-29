using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using FranoW.DevelopTools;
using UnityEngine.UI;

public class MyServer_FA : MonoBehaviourPun
{
    public static MyServer_FA Instance;

    Player _server;
    
    public LobbyController_FA lobby;
    public Controller_FA controller_pf;
    public Spawners_FA spawner;
    public UIController_FA UI_controller;
    public GameController_FA gameController;
    public EventManager eventManager = new EventManager();

    Dictionary<Player, Character_FA> _dicModels = new Dictionary<Player, Character_FA>();
    Dictionary<Player, CharSelect_FA> _dicSelectionModel = new Dictionary<Player, CharSelect_FA>();
    Dictionary<Player, CharView_FA> _dicViews = new Dictionary<Player, CharView_FA>();
    Dictionary<Player, bool> _dicEnterLobby = new Dictionary<Player, bool>();
    Dictionary<Player, LobbySelectorData> _dicCharacterLobbyData = new Dictionary<Player, LobbySelectorData>();

    public List<LobbySelectorData> lobySelectorDatas = new List<LobbySelectorData>();

    int playersConnected = 0;
    int playersReadyToPlay = 0;
    int playersNeededToPlay = 1;
    public int PackagePerSecond { get; private set; }
    public Player GetServer => _server;

    public Dictionary<Player, LobbySelectorData> GetCharacterLobbyDataDictionary => _dicCharacterLobbyData;
    
    #region ServerInitializer

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if(Instance == null)
        {
            if(photonView.IsMine)
            {
                photonView.RPC("SetServer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer, 1);   
                StartCoroutine(WaitForLevel(() => lobby = FindObjectOfType<LobbyController_FA>()));
            }
        }
    }

    [PunRPC]
    void SetServer(Player serverPlayer, int sceneIndex = 1)
    {
        if(Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        _server = serverPlayer;

        PackagePerSecond = 60;

        PhotonNetwork.LoadLevel(sceneIndex);

        var playerLocal = PhotonNetwork.LocalPlayer;
    }

    #endregion

    #region lobbyLoadSettings
    IEnumerator WaitForLevel(Action callback)
    {
        while (PhotonNetwork.LevelLoadingProgress > 0.99f)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(2);
        callback?.Invoke();
    }
    public void RequestEnterLobby(Player player)
    {
        photonView.RPC("RPCEnterLobby", _server, player);
    }

    public void RequestEnterLobbyAgain(Player player)
    {
        photonView.RPC("RPCEnterLobbyAgain", _server, player);
    }
    
    void SetGameplay(Player player)
    {
        PhotonNetwork.LoadLevel(2);

        StartCoroutine(WaitForLevel(() => {
            spawner = FindObjectOfType<Spawners_FA>();
            UI_controller = FindObjectOfType<UIController_FA>();
            gameController = FindObjectOfType<GameController_FA>();
        }));

        StartCoroutine(WaitForLevelSettings(() => spawner != null, () => photonView.RPC("StartGame", RpcTarget.Others)));
    }

    public void RequestCreateModelPlayer(Player player)
    {
        photonView.RPC("CreatePlayerModel", _server, player);
    }
    
    IEnumerator WaitForLevelSettings(Func<bool> condition, Action callback)
    {
        while (!condition())
        {
            yield return new WaitForEndOfFrame();
        }

        callback?.Invoke();
    }

    // // // ---- RPCs ---- \\ \\ \\
    
    [PunRPC]
    void SetInitialParams(Player player, int playerIndex)
    {
        StartCoroutine(WaitForLevel(() => lobby.characterSelections[playerIndex].SetInitialParams(playerIndex)));
    }
    
    [PunRPC]
    void RPCEnterLobby(Player player)
    {
        Debug.Log("registre a un jugador");
        
        lobby.SetInitialParams(player, playersConnected);
        _dicSelectionModel.Add(player, lobby.characterSelections[playersConnected]);
        _dicCharacterLobbyData.Add(player, lobySelectorDatas[playersConnected]);
        //lobby.SetInitialView(playersConnected, player);


        //Debug.Log("soy el jugador " + _dicSelectionModel[player].playerIndex);

        playersConnected++;
    }

    [PunRPC]
    void RPCEnterLobbyAgain(Player player)
    {
        if (_dicEnterLobby.ContainsKey(player)) return;

        _dicEnterLobby[player] = true;

        photonView.RPC("RPC_ReloadLobbyAndEnterPlayer", player);
        StartCoroutine(WaitForLevel(() => { lobby = FindObjectOfType<LobbyController_FA>(); }));
    }

    [PunRPC]
    void RPC_ReloadLobbyAndEnterPlayer()
    {
        Debug.Log("el jugador " + PhotonNetwork.LocalPlayer.NickName + " va a entrar al lobby");
        PhotonNetwork.LoadLevel(1);

        StartCoroutine(WaitForLevel(() => photonView.RPC("RPCEnterLobby", _server, PhotonNetwork.LocalPlayer)));
    }

    void ClearSettings()
    {
        _dicModels.Clear();
        _dicSelectionModel.Clear();
        _dicViews.Clear();
        playersConnected = 0;
        playersReadyToPlay = 0;
        _dicEnterLobby.Clear();
    }
    
    #endregion

    #region ActionsInLobby

    [PunRPC]
    void ReadyToPlay(Player player)
    {
        if (_dicSelectionModel[player].imReady == true)
        {
            NOT_ReadyToPlay(player);
            return;
        }

        Debug.Log(_dicSelectionModel[player].playerIndex + " esta listo");

        _dicSelectionModel[player].imReady = true;
        lobby.RequestRefreshView(_dicSelectionModel[player].playerIndex, player, true);

        playersReadyToPlay++;

        Debug.Log("entra aca 4" + playersReadyToPlay);

        if (playersReadyToPlay == playersNeededToPlay)
        {
            
            SetGameplay(player);
        }
    }
    
    public void RequestReadyToPlay(Player player)
    {
        photonView.RPC("ReadyToPlay", _server, player);
    }

    public void Request_NOT_ReadyToPlay(Player player)
    {
        photonView.RPC("NOT_ReadyToPlay", _server, player);
    }
    
    [PunRPC]
    void StartGame()
    {
        Debug.Log("Arranca el juego");
        
        PhotonNetwork.LoadLevel(2);
    }

    [PunRPC]
    void NOT_ReadyToPlay(Player player)
    {
        Debug.Log(_dicSelectionModel[player].playerIndex + " dejo de estar listo");
        _dicSelectionModel[player].imReady = false;
        lobby.RequestRefreshView(_dicSelectionModel[player].playerIndex, player, false);
        
        playersReadyToPlay--;

        Debug.Log("entra aca 5" + playersReadyToPlay);
    }

    public void RefreshPlayerLobbyData(int index)
    {
        photonView.RPC("RPC_RefreshPlayerLobbyData", _server, index, PhotonNetwork.LocalPlayer);
    }

    [PunRPC]
    void RPC_RefreshPlayerLobbyData(int index, Player player)
    {
        Debug.Log("el server registra");
        _dicCharacterLobbyData[player] = lobySelectorDatas[index];
        
        Debug.Log("el server tiene " + _dicCharacterLobbyData.Count);
    }
    
    #endregion
    
    #region Player actions (van al gamecontroller)

    public void RequestDash(Player localPlayer)
    {
        photonView.RPC("RPC_DoDash", _server, localPlayer);
    }

    [PunRPC]
    void RPC_DoDash(Player player)
    {
        if (_dicModels.ContainsKey(player))
        {
            _dicModels[player].Dash();
        }
    }

    public void RequestStopMovement(Player localPlayer)
    {
        photonView.RPC("RPC_StopMovement", _server, localPlayer);
    }
    
    [PunRPC]
    void RPC_StopMovement(Player player)
    {
        if (_dicModels.ContainsKey(player))
        {
            _dicModels[player].StopMovement();
        }
    }

    public void RequestResumeMovement(Player localPlayer)
    {
        photonView.RPC("RPC_ResumeMovement", _server, localPlayer);
    }
    
    [PunRPC]
    void RPC_ResumeMovement(Player player)
    {
        if (_dicModels.ContainsKey(player))
        {
            _dicModels[player].ResumeMovement();
        }
    }
    
    public void OnTouchedTrap(Player player)
    {
        _dicModels[player].ResetCharacter(Vector3.zero);
    }
    
    public void RequestMove(Player player, Vector3 dir)
    {
        photonView.RPC("Move", _server, player, dir);
    }

    public void RequestRotation(Player localPlayer, float xRotation, float mouseX)
    {
        photonView.RPC("RPC_RequestRotation", _server, localPlayer, xRotation, mouseX);
    }
    
    [PunRPC]
    void Move(Player player, Vector3 dir)
    {
        if (_dicModels.ContainsKey(player))
        {
            _dicModels[player].Move(dir, _dicModels[player].speed);
        }
    }

    [PunRPC]
    void RPC_RequestRotation(Player player, float xRotation, float mouseX)
    {
        if (_dicModels.ContainsKey(player))
        {
            _dicModels[player].Rotate(xRotation, mouseX);
        }
    }
    
    [PunRPC]
    void CreatePlayerModel(Player player)
    {
        _dicModels[player] = PhotonNetwork.Instantiate("RatTest", spawner.transform.position, Quaternion.identity).GetComponent<Character_FA>().SetInitialParameters(player);
        gameController.AddModel(player, _dicModels[player]); // lo agrego al controlador del juego
        UI_controller.RegisterPlayerUI(player);
    }
    
    [PunRPC]
    void RPCCheckIfEndGame(Player player)
    {
        if (gameController.IsGameFinished())
        {
            ClearSettings();
            PhotonNetwork.LoadLevel(1);

            StartCoroutine(WaitForLevel(() => {
                lobby = FindObjectOfType<LobbyController_FA>();
                photonView.RPC("RPC_RequestEnterLobbyFromGame", RpcTarget.Others);
            }));
        }
        
    }

    public void ReloadLobby()
    {
        ClearSettings();
        PhotonNetwork.LoadLevel(1);

        StartCoroutine(WaitForLevel(() => {
            lobby = FindObjectOfType<LobbyController_FA>();
        }));
        
    }

    [PunRPC]
    void RPC_RequestEnterLobbyFromGame()
    {
        RequestEnterLobbyAgain(PhotonNetwork.LocalPlayer);
    }

    #endregion

    #region UI

    void FinishGame()
    {
        ClearSettings();
    }
    
    
    #endregion
}
