using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForEveryoneToUnpause : MonoBehaviour
{
    private void Start()
    {
        Hide();
        GameManager_.Instance.OnGameNetworkPaused += GameManager_OnGameNetworkPaused;
        GameManager_.Instance.OnGameNetworkUnpaused += GameManager_OnGameNetworkUnpaused;
    }

    private void GameManager_OnGameNetworkPaused(object sender, EventArgs e)
    {
        Show();

    }

    private void GameManager_OnGameNetworkUnpaused(object sender, EventArgs e)
    {
        Hide();
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
