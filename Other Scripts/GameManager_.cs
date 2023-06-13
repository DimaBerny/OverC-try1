using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_ : NetworkBehaviour
{

    [SerializeField] private Transform playerPrefab;
    public static GameManager_ Instance { get; private set; }
    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalPause;
    public event EventHandler OnLocalUnpause;
    public event EventHandler OnGameNetworkPaused;
    public event EventHandler OnGameNetworkUnpaused;
    public event EventHandler OnLocalPlayerReadyChanged;


    private enum State
    {
        WaitToStart,
        CountdownToStart,
        Playing,
        GameOver,
    }

    //default setting is wait to start
    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitToStart);
    private NetworkVariable<float> coundownTimerStart = new NetworkVariable<float>(1.5f);
    private NetworkVariable<float> playingTimer = new NetworkVariable<float>(0f);
    private float playingTimerMax = 180f;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    //only for locals but for all is network
    private bool isLocalGamePaused = false;
    private bool isLocalPlayerReady = false;
    private bool isDisconnectedAndPaused = false;
    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPausedDictionary;


    private void Awake()
    {
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPausedDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnEscapePressed += Game_OnPause;
            Player.LocalInstance.OnReadyPressed += Game_PlayerReady;
        }
        else
        {
            StartCoroutine(WaitForPlayerSpawn());
        }
    }

    private IEnumerator WaitForPlayerSpawn()
    {
        while (Player.LocalInstance == null)
        {
            yield return null;
        }

        Player.LocalInstance.OnEscapePressed += Game_OnPause;
        Player.LocalInstance.OnReadyPressed += Game_PlayerReady;
    }


    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += isGamePaused_OnValueChanged;
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetwworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCopleted;
        }
    }

    private void NetwworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        isDisconnectedAndPaused = true;
    }

    private void SceneManager_OnLoadEventCopleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void isGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        //so if game paused Time magika
        if (isGamePaused.Value)
        {
            Time.timeScale = 0f;

            OnGameNetworkPaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;

            OnGameNetworkUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Game_OnPause(object sender, EventArgs e)
    {
        Pause();
    }
    private void Game_PlayerReady(object sender, EventArgs e)
    {
        if (state.Value == State.WaitToStart)
        {
            isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                //Player NOT ready
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            state.Value = State.CountdownToStart;
        }
    }



    private void Update()
    {
        if (!IsServer) { return; }


        switch (state.Value)
        {
            case State.WaitToStart:
                // waitTimerStart -= Time.deltaTime;
                // if (waitTimerStart < 0f && Input.GetKeyDown(KeyCode.Space))
                // {
                //     state = State.CountdownToStart;
                //     coundownTimerStart = 1f; // Присвоїти значення countdownTimerStart
                //     OnStateChanged?.Invoke(this, EventArgs.Empty);
                // }
                break;
            case State.CountdownToStart:
                coundownTimerStart.Value -= Time.deltaTime;
                if (coundownTimerStart.Value < 0f)
                {
                    state.Value = State.Playing;
                    playingTimer.Value = playingTimerMax;

                }
                break;
            case State.Playing:
                playingTimer.Value -= Time.deltaTime;
                if (playingTimer.Value < 0f)
                {
                    state.Value = State.GameOver;

                }
                break;
            case State.GameOver:
                break;
        }
    }

    private void LateUpdate()
    {
        if (isDisconnectedAndPaused)
        {
            isDisconnectedAndPaused = false;
            CheckIfGamePaused();
        }

    }

    public bool IsPlaying()
    {
        return state.Value == State.Playing;
    }

    public float GetCountdownToStart()
    {
        return coundownTimerStart.Value;
    }

    public bool IsCountdownToStart()
    {
        return state.Value == State.CountdownToStart;
    }


    public bool IsGameOver()
    {
        return state.Value == State.GameOver;
    }
    public bool IsWaitToStart()
    {
        return state.Value == State.WaitToStart;
    }

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

    //for clock
    public float GetPlayingTimerNormalized()
    {
        return 1 - (playingTimer.Value / playingTimerMax);
    }

    public void Pause()
    {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused)
        {
            //activate pause ui
            OnLocalPause?.Invoke(this, EventArgs.Empty);
            PauseServerRpc();
        }
        else
        {
            //deactivate pause ui
            OnLocalUnpause?.Invoke(this, EventArgs.Empty);
            UnpauseServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void PauseServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;

        CheckIfGamePaused();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnpauseServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;

        CheckIfGamePaused();
    }

    private void CheckIfGamePaused()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId])
            {
                // player in pause
                isGamePaused.Value = true;
                return;
            }
        }

        // players are unpaused
        isGamePaused.Value = false;
    }

}