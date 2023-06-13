using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button mainMenu;
    [SerializeField] private TextMeshProUGUI completedOrders;

    private void Start()
    {
        GameManager_.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }
    private void Awake()
    {
        mainMenu.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            LoadListOrder.LoadNetwork(LoadListOrder.Scene.MainMenuScene);
        });
    }
    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {

        if (GameManager_.Instance.IsGameOver())
        {
            Show();
            completedOrders.text = DeliveryManager.Instance.GetCompletedOrders().ToString();
        }
        else
        {
            Hide();
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
}
