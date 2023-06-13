using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MainMenuDeleteOpened : MonoBehaviour
{
    // all of this exists that every object with DodntDestroyOnLoad has been destroyed if we are in main menu
    private void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
        if (GameNetworkConnector.Instance != null)
        {
            Destroy(GameNetworkConnector.Instance.gameObject);
        }
        if (GameLobby.Instance != null)
        {
            Destroy(GameLobby.Instance.gameObject);
        }
    }
}
