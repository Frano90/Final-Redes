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
    //public ControllerBase_FA controllerBasePf;
    public Spawners_FA spawner;
    public UIController_FA UI_controller;
    public GameController_FA gameController;
    public EventManager eventManager = new EventManager();

    Dictionary<Player, Character_FA> _dicModels = new Dictionary<Player, Character_FA>();
    Dictionary<Player, bool> _dicPlayersReadyToPlay = new Dictionary<Player, bool>();
    Dictionary<Player, bool> _dicEnterLobby = new Dictionary<Player, bool>();
    Dictionary<Player, LobbySelectorData> _dicCharacterLobbyData = new Dictionary<Player, LobbySelectorData>();

    public List<LobbySelectorData> lobySelectorDatas = new List<LobbySelectorData>();

    int playersConnected = 0;
    int playersReadyToPlay = 0;
    int playersNeededToPlay = 2;

    public int PlayersConnected
    {
        get => playersConnected;
        set => playersConnected = value;
    }

    public int PackagePerSecond { get; private set; }
    public Player GetServer => _server;

    public Dictionary<Player, Character_FA> GetModels => _dicModels;
    public Dictionary<Player, LobbySelectorData> GetCharacterLobbyDataDictionary => _dicCharacterLobbyData;
    public Dictionary<Player, bool> GetPlayersReadyDictionary => _dicPlayersReadyToPlay;
    
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

        lobby.RegisterLocalData(player, playersConnected);

        _dicPlayersReadyToPlay.Add(player, false);
        _dicCharacterLobbyData.Add(player, lobySelectorDatas[playersConnected]);

        playersConnected++;
        
        lobby.RefreshAllClientsViews();
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
        //Debug.Log("el jugador " + PhotonNetwork.LocalPlayer.NickName + " va a entrar al lobby");
        PhotonNetwork.LoadLevel(1);

        StartCoroutine(WaitForLevel(() => photonView.RPC("RPCEnterLobby", _server, PhotonNetwork.LocalPlayer)));
    }

    void ClearSettings()
    {
        _dicModels.Clear();
        _dicPlayersReadyToPlay.Clear();
        playersConnected = 0;
        playersReadyToPlay = 0;
        _dicCharacterLobbyData.Clear();
        _dicEnterLobby.Clear();
    }
    
    #endregion

    #region ActionsInLobby

    [PunRPC]
    void RPC_ReadyToPlay(Player player)
    {
        if (_dicPlayersReadyToPlay[player])
        {
            _dicPlayersReadyToPlay[player] = false;
            return;
        }

        _dicPlayersReadyToPlay[player] = true;
        
        if (playersConnected != playersNeededToPlay) return;
        
        foreach (bool pValue in _dicPlayersReadyToPlay.Values)
        {
            if(!pValue) return;
        }

        int cats = 0;
        int rats = 0;
        
        foreach (var playerTeam in _dicCharacterLobbyData.Values)
        {
            if (playerTeam.team == LobbySelectorData.Team.cat) cats++;
            
            if (playerTeam.team == LobbySelectorData.Team.rat) rats++;
        }

        Debug.Log("hay " + cats + " gatos");
        Debug.Log("hay " + rats + " ratas");
        
        if (cats != 1 && rats != 3)
        {
            lobby.ShowPanel(LobbyController_FA.LobbyPanelType.NotCorrectTeams);
            return;
        }
        
        lobby.ShowPanel(LobbyController_FA.LobbyPanelType.StartingGame);
        StartCoroutine(WaitAndStartGame(player));
    }

    IEnumerator WaitAndStartGame(Player player)
    {
        yield return new WaitForSeconds(.5f);
            
            SetGameplay(player);
    }
    
    
    public void RequestReadyToPlay(Player player)
    {
        photonView.RPC("RPC_ReadyToPlay", _server, player);
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
    public void RefreshPlayerLobbyData(Player player)
    {
        for (int i = 0; i < lobySelectorDatas.Count; i++)
        {
            if (_dicCharacterLobbyData[player].Equals(lobySelectorDatas[i]))
            {
                if (i + 1 >= lobySelectorDatas.Count)
                {
                    _dicCharacterLobbyData[player] = lobySelectorDatas[0];
                  
                }
                else
                {
                    _dicCharacterLobbyData[player] = lobySelectorDatas[i + 1];
        
                }
                break;
            }
        }
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
        _dicModels[player].ResetCharacter();
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
        //chequear de que equipo es antes de spawnear
        
        //_dicModels[player] = PhotonNetwork.Instantiate("RatTest", spawner.transform.position, Quaternion.identity).GetComponent<Character_FA>(), ;
        //gameController.AddModel(player, _dicModels[player]); // lo agrego al controlador del juego
        //UI_controller.RegisterPlayerUI(player);
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

    public void RequestJump(Player localPlayer)
    {
        photonView.RPC("RCP_StartJump", _server, localPlayer);
    }

    [PunRPC]
    void RCP_StartJump(Player localPlayer)
    {
        if (_dicModels.ContainsKey(localPlayer))
        {
            //Aca solo deberia entrar el gato, asi que casteo tranqui
            var catPlayerModel = _dicModels[localPlayer] as CatCharacter_FA;

            if(!catPlayerModel.grounded) return;
            
            catPlayerModel.StartJump();
        }
    }

    public void RequestMoveCameraOnWait(Player localPlayer, float value)
    {
        photonView.RPC("RCP_MoveCameraOnWait", _server, localPlayer, value);
    }
    
    [PunRPC]
    void RCP_MoveCameraOnWait(Player localPlayer, float value)
    {
        if (_dicModels.ContainsKey(localPlayer))
        {
            //Aca solo deberia entrar el gato, asi que casteo tranqui
            var catPlayerModel = _dicModels[localPlayer] as CatCharacter_FA;

            if(!catPlayerModel.isWaitingJump) return;
            
            catPlayerModel.MoveCameraOnWait(value);
        }
    }

    public void RequestCamBackToPos(Player localPlayer)
    {
        photonView.RPC("RCP_CamBackToPos", _server, localPlayer);
    }
    
    [PunRPC]
    void RCP_CamBackToPos(Player localPlayer)
    {
        if (_dicModels.ContainsKey(localPlayer))
        {
            //Aca solo deberia entrar el gato, asi que casteo tranqui
            var catPlayerModel = _dicModels[localPlayer] as CatCharacter_FA;

            catPlayerModel.CamBackToPos();
        }
    }

    public void RequestReleaseJump(Player localPlayer)
    {
        photonView.RPC("RCP_ReleaseJump", _server, localPlayer);
    }
    
    [PunRPC]
    void RCP_ReleaseJump(Player localPlayer)
    {
        if (_dicModels.ContainsKey(localPlayer))
        {
            //Aca solo deberia entrar el gato, asi que casteo tranqui
            var catPlayerModel = _dicModels[localPlayer] as CatCharacter_FA;

            if(!catPlayerModel.isWaitingJump) return;
            
            catPlayerModel.ReleaseJump();
        }
    }
}
