using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        GameNetworkConnector.Instance.OnTryJoinTheGame += GameNetworkConnector_OnTryJoinTheGame;
        GameNetworkConnector.Instance.OnFailJoinTheGame += GameNetworkConnector_OnFailJoinTheGame;
        Hide();
    }

    private void GameNetworkConnector_OnFailJoinTheGame(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameNetworkConnector_OnTryJoinTheGame(object sender, EventArgs e)
    {
        Show();
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
        GameNetworkConnector.Instance.OnTryJoinTheGame -= GameNetworkConnector_OnTryJoinTheGame;
        GameNetworkConnector.Instance.OnFailJoinTheGame -= GameNetworkConnector_OnFailJoinTheGame;
    }
}
