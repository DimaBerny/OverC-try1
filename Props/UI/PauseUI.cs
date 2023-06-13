using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private Button mainMenu;
    private void Start()
    {
        GameManager_.Instance.OnLocalPause += GameMan_OnLocalPause;
        GameManager_.Instance.OnLocalUnpause += GameMan_OnLocalUnPause;
        Hide();
    }

    private void GameMan_OnLocalUnPause(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameMan_OnLocalPause(object sender, EventArgs e)
    {
        Show();
    }

    private void Awake()
    {
        mainMenu.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            LoadListOrder.LoadNetwork(LoadListOrder.Scene.MainMenuScene);
        });
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
