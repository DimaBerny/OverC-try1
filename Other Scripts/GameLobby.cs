using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLobby : MonoBehaviour
{
    private const string KEY_relay_join_code = "RelayJoinCode";
    public static GameLobby Instance { get; private set; }
    private Lobby joinedLobby;
    private float heartBeatTimer;
    private void Awake()
    {
        Instance = this;
        //we need lobby logic in the character select so we cant destroy it after changing scenes
        DontDestroyOnLoad(gameObject);
        // Initialize Unity Authentication
        InitializeUnityAutentication();
    }


    //lobby auto destroys themself after 30 seconds, so to keep them alive we have to make "Heartbeats"
    private void Update()
    {
        HandleHeartbeat();
    }

    private void HandleHeartbeat()
    {
        if (IsLobbyHost())
        {
            heartBeatTimer -= Time.deltaTime;
            if (heartBeatTimer <= 0f)
            {
                float heartBeatTimerMax = 15f;
                heartBeatTimer = heartBeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }
    private bool IsLobbyHost()
    {
        //is lobby active AND check if the host ID of the lobby matches the ID of the current player
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void InitializeUnityAutentication()
    {
        // Check if Unity Services are already initialized
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            // Create initialization options for Unity Services
            // Set a random profile for the user
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 1000).ToString());

            // Initialize Unity Services asynchronously -- 
            //--means: Instead of waiting for each step to complete before moving on to the next, 
            //  the initialization is performed in the background 
            //  while allowing the rest of the program to continue executing.
            await UnityServices.InitializeAsync(initializationOptions);
            // Initialize Unity Services asynchronously
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, GameNetworkConnector.Max_quantity_players, new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
            });

            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);
            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                     { KEY_relay_join_code , new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                 }
            });
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            GameNetworkConnector.Instance.StartHost();
            LoadListOrder.LoadNetwork(LoadListOrder.Scene.SelectYourCharacter);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }
    public async void QuiickJoin()
    {
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = joinedLobby.Data[KEY_relay_join_code].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));



            GameNetworkConnector.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    /////////////////////////////////////////////////////////
    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(GameNetworkConnector.Max_quantity_players - 1);

            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }


    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            return relayJoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }
    /////////////////////////////////////////////////////////

    public async void JoinCode(string lobbyCode)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            string relayJoinCode = joinedLobby.Data[KEY_relay_join_code].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            GameNetworkConnector.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void DeleteLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }
}
