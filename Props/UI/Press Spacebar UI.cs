using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PressSpacebarUI : MonoBehaviour
{
    private void Start()
    {
        //GameManager_.Instance.OnStateChanged += GameManager_OnStateChanged;
        GameManager_.Instance.OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerReadyChanged;
    }
    // private void GameManager_OnStateChanged(object sender, EventArgs e)
    // {

    //     if (GameManager_.Instance.IsWaitToStart())
    //     {
    //         Show();
    //     }
    //     else
    //     {
    //         Hide();
    //     }
    // }
    private void GameManager_OnLocalPlayerReadyChanged(object sender, EventArgs e)
    {
        if (GameManager_.Instance.IsLocalPlayerReady())
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
