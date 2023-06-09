using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            LoadListOrder.LoadNetwork(LoadListOrder.Scene.MainMenuScene);
        });

        readyButton.onClick.AddListener(() =>
        {
            CharacterSelecReady.Instance.SetPlayerReady();
        });
    }

    private void Start()
    {
        Lobby lobby = GameLobby.Instance.GetLobby();
        lobbyNameText.text = lobby.Name;
        lobbyCodeText.text = lobby.LobbyCode;
    }
}
