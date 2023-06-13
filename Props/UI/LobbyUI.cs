using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenu;
    [SerializeField] private Button createLobby;
    [SerializeField] private Button joinLobby;
    [SerializeField] private CreateLobbyUI createLobbyUI;
    [SerializeField] private Button joinCode;
    [SerializeField] private TMP_InputField joinCodeInputField;

    private void Awake()
    {
        mainMenu.onClick.AddListener(() =>
        {
            GameLobby.Instance.LeaveLobby();
            LoadListOrder.Load(LoadListOrder.Scene.MainMenuScene);
        });
        createLobby.onClick.AddListener(() =>
        {
            createLobbyUI.Show();
        });

        joinLobby.onClick.AddListener(() =>
        {
            GameLobby.Instance.QuiickJoin();
        });
        joinCode.onClick.AddListener(() =>
        {
            GameLobby.Instance.JoinCode(joinCodeInputField.text);
        });
    }
}
