using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;

//name is a bit wrong this script will fix a bit more than just spawn
public class GameNetworkConnector : NetworkBehaviour
{
    public const int Max_quantity_players = 4;
    //reference to every prop SO
    //create id so pick up objects by indexes that server understand it
    [SerializeField] private PropSortListSO propSOList;
    [SerializeField] private List<Color> playerBodyColorList;
    //PlayerData is a STRUCT script. 
    private NetworkList<PlayerData> playerDataNetworkList;

    public static GameNetworkConnector Instance { get; private set; }
    public event EventHandler OnPlayerDataNetworkListChanged;
    public event EventHandler OnTryJoinTheGame;
    public event EventHandler OnFailJoinTheGame;

    public void StartHost()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;

            NetworkManager.Singleton.StartHost();
        }
    }

    //when dixconected. Cycle through all of clients. When found one, we remove him from lobby
    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerdata = playerDataNetworkList[i];
            if (playerdata.clientId == clientId)
            {
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId,
            colorId = GetFirstUnusedColorId(),
        });
    }
    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {

        if (SceneManager.GetActiveScene().name != LoadListOrder.Scene.SelectYourCharacter.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Cannot join active game";
            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= Max_quantity_players)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }
        connectionApprovalResponse.Approved = true;
    }

    public void StartClient()
    {
        OnTryJoinTheGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientDisconnectedCallback(ulong clientId)
    {
        OnFailJoinTheGame?.Invoke(this, EventArgs.Empty);
    }

    private void Awake()
    {
        Instance = this;
        //changing scenes will not destroy this object
        DontDestroyOnLoad(gameObject);

        //Initialize list 
        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    // Spawns a prop with the specified propSO and assigns it to the propParent
    public void SpawnProp(PropsSort propSO, I_PropParent propParent)
    {
        // Call the SpawnPropServerRpc on the server and pass the propSO index and propParent's NetworkObject
        SpawnPropServerRpc(GetPropSONumb(propSO), propParent.GetNetworkObject());
    }

    // ServerRpc method that spawns a prop based on the provided propSO index and assigns it to the propParent
    [ServerRpc(RequireOwnership = false)]
    private void SpawnPropServerRpc(int propSONumb, NetworkObjectReference propParentNetworkObjectReference)
    {
        // Retrieve the PropsSort object from the propSOList using the provided index
        PropsSort propsSort = GetPropsSortFromNumb(propSONumb);

        propParentNetworkObjectReference.TryGet(out NetworkObject propParentNetworkObject);
        I_PropParent propParent = propParentNetworkObject.GetComponent<I_PropParent>();

        // Instantiate the prop's prefab and get its NetworkObject component
        Transform propTransform = Instantiate(propsSort.prefab);
        NetworkObject propNetworkObject = propTransform.GetComponent<NetworkObject>();

        // Spawn the prop on the network
        propNetworkObject.Spawn(true);

        // Get the Props component from the spawned prop and set its propParent to the provided propParent
        Props prop = propTransform.GetComponent<Props>();

        prop.SetPropParent(propParent);
    }

    // Get the index of the provided PropsSort object in the propSOList
    public int GetPropSONumb(PropsSort propSO)
    {
        return propSOList.propSOList.IndexOf(propSO);
    }

    // Get the PropsSort object from the propSOList using the provided index
    public PropsSort GetPropsSortFromNumb(int propSONumb)
    {
        return propSOList.propSOList[propSONumb];
    }

    //Despawn fix
    public void DespawnProp(Props prop)
    {
        DespawnPropServerRpc(prop.NetworkObject);
    }
    //It receives a reference to the prop's network object, tries to obtain the NetworkObject and Props components, and assigns them to the local variables propNetworkObject and prop, respectively. From here, we can perform any necessary actions on the Props component, such as deactivating or destroying it.
    [ServerRpc(RequireOwnership = false)]
    private void DespawnPropServerRpc(NetworkObjectReference propNetworkObjectReference)
    {
        propNetworkObjectReference.TryGet(out NetworkObject propNetworkObject);
        Props prop = propNetworkObject.GetComponent<Props>();

        ClearPropParentClientRpc(propNetworkObjectReference);

        prop.Despawn();
    }
    //on client Unparenting. On server destroying children
    [ClientRpc]
    private void ClearPropParentClientRpc(NetworkObjectReference propNetworkObjectReference)
    {
        propNetworkObjectReference.TryGet(out NetworkObject propNetworkObject);
        Props prop = propNetworkObject.GetComponent<Props>();

        prop.ClearPropParent();
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorServerRpc(colorId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsColorAvailable(colorId))
        {
            // we cannot pick it
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.colorId = colorId;

        playerDataNetworkList[playerDataIndex] = playerData;

    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    private bool IsColorAvailable(int colorId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.colorId == colorId)
            {
                // In use, so we cannot pick it
                return false;
            }
        }
        return true;
    }


    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }
    private int GetFirstUnusedColorId()
    {
        for (int i = 0; i < playerBodyColorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }
        return -1;
    }
    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }
    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public Color GetPlayerColor(int colorId)
    {
        return playerBodyColorList[colorId];
    }
}