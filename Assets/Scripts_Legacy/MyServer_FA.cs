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

    public Character_FA characterTest_PF;
    //public Character_FA character_pf_BLUE;
    //public LobbyController_FA lobby_pf;
    public LobbyController_FA lobby;
    //public CharSelect_FA charSelect_pf;
    public Controller_FA controller_pf;
    public Spawners_FA spawner;
    public UIController_FA UI_controller;
    public GameController_FA gameController;
    public EventManager eventManager = new EventManager();

    Dictionary<Player, Character_FA> _dicModels = new Dictionary<Player, Character_FA>();
    Dictionary<Player, CharSelect_FA> _dicSelectionModel = new Dictionary<Player, CharSelect_FA>();
    Dictionary<Player, CharView_FA> _dicViews = new Dictionary<Player, CharView_FA>();
    Dictionary<Player, Team> _dicTeams = new Dictionary<Player, Team>();
    Dictionary<Player, bool> _dicEnterLobby = new Dictionary<Player, bool>();

    int playersConnected = 0;
    int playersReadyToPlay = 0;
    int playersNeededToPlay = 1;

    public bool isGameOn = false;

    public int PackagePerSecond { get; private set; }

    public enum Team { Yellow, Blue, NotAssigned}

    public Player GetServer => _server;

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
    

    #region lobbyShit

    [PunRPC]
    void SetInitialParams(Player player, int playerIndex)
    {
        StartCoroutine(WaitForLevel(() => lobby.characterSelections[playerIndex].SetInitialParams(playerIndex)));
    }

    [PunRPC]
    void ChangeLobbyName(Player player, int playerIndex)
    {
        StartCoroutine(WaitForLevel(() => { lobby.characterSelections[playerIndex].ChangeName(player.NickName); }));
    }


    IEnumerator WaitForLevel(Action callback)
    {
        while (PhotonNetwork.LevelLoadingProgress > 0.99f)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(2);
        callback?.Invoke();
    }

    IEnumerator WaitForLevel(Action callback, float time)
    {
        while (PhotonNetwork.LevelLoadingProgress > 0.99f)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(time);
        callback?.Invoke();
    }

    IEnumerator WaitForLevel(Action callback, Func<bool> condition)
    {
        while (PhotonNetwork.LevelLoadingProgress > 0.99f)
        {
            yield return new WaitForEndOfFrame();
        }

        while (!condition())
        {
            yield return new WaitForEndOfFrame();
        }
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

    public void RequestChangeTeam(Player player)
    {
        photonView.RPC("ChangeTeam", _server, player);
    }

    public void RequestReadyToPlay(Player player)
    {
        photonView.RPC("ReadyToPlay", _server, player);
    }

    public void Request_NOT_ReadyToPlay(Player player)
    {
        photonView.RPC("NOT_ReadyToPlay", _server, player);
    }

    void SetGameplay(Player player)
    {
        PhotonNetwork.LoadLevel(2);

        StartCoroutine(WaitForLevel(() => {
            spawner = FindObjectOfType<Spawners_FA>();
            UI_controller = FindObjectOfType<UIController_FA>();
            gameController = FindObjectOfType<GameController_FA>();
            isGameOn = true;
        }));

        StartCoroutine(WaitForLevelSettings(() => spawner != null, () => photonView.RPC("StartGame", RpcTarget.Others)));
    }

    public void RequestCreateModelPlayer(Player player)
    {
        photonView.RPC("CreatePlayerModel", _server, player);
    }

    // // // ---- RPCs ---- \\ \\ \\
    [PunRPC]
    void RPCEnterLobby(Player player)
    {
        Debug.Log("registre a un jugador");
        
        lobby.SetInitialParams(player, playersConnected);
        _dicSelectionModel.Add(player, lobby.characterSelections[playersConnected]);

        lobby.SetInitialView(playersConnected, player);


        Debug.Log("soy el jugador " + _dicSelectionModel[player].playerIndex);

        playersConnected++;
    }

    [PunRPC]
    void RPCEnterLobbyAgain(Player player)
    {
        Debug.Log("ya entaste? " + _dicEnterLobby.ContainsKey(player));
        if (_dicEnterLobby.ContainsKey(player)) return;

        _dicEnterLobby[player] = true;

        Debug.Log("logre meterme aca. Voy a cambiar de escena");
        //if(_dicEnterLobby.Count == playersNeededToPlay)
        //{
            //PhotonNetwork.LoadLevel(1);
            photonView.RPC("RPC_ReloadLobbyAndEnterPlayer", player);
            StartCoroutine(WaitForLevel(() => { lobby = FindObjectOfType<LobbyController_FA>(); }));            
        //}
    }

    [PunRPC]
    void RPC_ReloadLobbyAndEnterPlayer()
    {
        Debug.Log("el jugador " + PhotonNetwork.LocalPlayer.NickName + " va a entrar al lobby");
        PhotonNetwork.LoadLevel(1);

        StartCoroutine(WaitForLevel(() => photonView.RPC("RPCEnterLobby", _server, PhotonNetwork.LocalPlayer)));
    }


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
            SetGameplay(player);
    }


    IEnumerator WaitForLevelSettings(Func<bool> condition, Action callback)
    {
        while (!condition())
        {
            yield return new WaitForEndOfFrame();
        }

        callback?.Invoke();
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
        ;
        playersReadyToPlay--;

        Debug.Log("entra aca 5" + playersReadyToPlay);
    }

    [PunRPC]
    void ChangeTeam(Player player)
    {

        if (_dicTeams[player] == Team.Blue)
        {
            _dicTeams[player] = Team.Yellow;
        }
        else
        {
            _dicTeams[player] = Team.Blue;
        }

        lobby.RequestRefreshView(_dicSelectionModel[player].playerIndex, player, _dicSelectionModel[player].imReady);
    }

    [PunRPC]
    void RefreshTeam(int playerIndex)
    {
        // if (lobby != null)
        //     StartCoroutine(WaitForLevel(() => lobby.characterSelections[playerIndex].ChangeTeam(), 1));

    }
    #endregion


    #region Player actions

    public void OnTouchedTrap(Player player)
    {
        _dicModels[player].ResetCharacter(Vector3.zero);
    }
    
    public void RequestMove(Player player, Vector3 dir)
    {
        Debug.Log("se hace el request");
        photonView.RPC("Move", _server, player, dir);
    }

    public void RequestWin(Player localPlayer)
    {
        photonView.RPC("RPCCheckIfEndGame", _server, localPlayer);
    }
    
    public void RequestRotation(Player localPlayer, float xRotation, float mouseX)
    {
        photonView.RPC("RPC_RequestRotation", _server, localPlayer, xRotation, mouseX);
    }
    
    [PunRPC]
    void Move(Player player, Vector3 dir)
    {
        //Lo dejo como ejemplo de como es la vuelta de photon
        
        if (_dicModels.ContainsKey(player))
        {
          
            // Debug.Log("trata de tirar raycast");
            // RaycastHit2D hit = Physics2D.Raycast(_dicModels[player].transform.position, dir, .1f, wall);
            //
            // if (hit.collider != null)
            // {
            //     _dicModels[player].Move(Vector2.zero, _dicModels[player].speed);
            //     return;
            // }
            //     
            //
            //
            //
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
    }
    
    [PunRPC]
    void RPCCheckIfEndGame(Player player)
    {
        //si la condicion de victoria se cumple...
        //podriamos tener algo como GameController.IsThereAWinner?
        //Gamecontroller.Stopgame

        if (gameController.IsGameFinished())
        {
            ClearSettings();
            PhotonNetwork.LoadLevel(1);

            //photonView.RPC("SetServer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer, 1);

            StartCoroutine(WaitForLevel(() => {
                lobby = FindObjectOfType<LobbyController_FA>();
                isGameOn = false;
                photonView.RPC("RPC_RequestEnterLobbyFromGame", RpcTarget.Others);
            }));

            //StartCoroutine(WaitForLevelSettings(() => lobby != null, () => photonView.RPC("SetServer", RpcTarget.Others, PhotonNetwork.LocalPlayer, 1)));
        }
        
    }

    [PunRPC]
    void RPC_RequestEnterLobbyFromGame()
    {
        Debug.Log("local player pide volver al lobby");
        RequestEnterLobbyAgain(PhotonNetwork.LocalPlayer);
    }

    #endregion


    #region UI 
    
    

    void FinishGame()
    {
        //spawner.RequestDeleteController();
        //PhotonNetwork.LoadLevel(2);
        ClearSettings();

        //StartCoroutine(WaitForLevel(Reset));        
    }
    #endregion

    void ClearSettings()
    {
        _dicModels.Clear();
        _dicSelectionModel.Clear();
        _dicTeams.Clear();
        _dicViews.Clear();
        isGameOn = false;
        playersConnected = 0;
        playersReadyToPlay = 0;
        _dicEnterLobby.Clear();
    }

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
}
