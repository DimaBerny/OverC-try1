using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public class ConnectResponseUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI responceText;
    [SerializeField] private Button backButton;
    private void Awake()
    {
        backButton.onClick.AddListener(() =>
        {
            Hide();
            NetworkManager.Singleton.Shutdown();
            LoadListOrder.LoadNetwork(LoadListOrder.Scene.MainMenuScene);
        });
    }
    private void Start()
    {
        GameNetworkConnector.Instance.OnFailJoinTheGame += GameNetworkConnector_OnFailJoinTheGame;
        Hide();
    }

    private void GameNetworkConnector_OnFailJoinTheGame(object sender, EventArgs e)
    {
        Show();
        responceText.text = NetworkManager.Singleton.DisconnectReason;
        if (responceText.text == "")
        {
            responceText.text = "Connection failed";
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }

    //UnSub from events cause wi will try to show/hide NULLs
    private void OnDestroy()
    {
        GameNetworkConnector.Instance.OnFailJoinTheGame -= GameNetworkConnector_OnFailJoinTheGame;
    }
}
