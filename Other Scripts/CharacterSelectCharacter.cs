using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectCharacter : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyTextGameObject;
    [SerializeField] private PlayerVisual playerVisual;
    private void Start()
    {
        GameNetworkConnector.Instance.OnPlayerDataNetworkListChanged += GameNetworkConnector_OnPlayerDataNetworkListChanged;
        CharacterSelecReady.Instance.OnReadyChanged += CharacterSelecReady_OnReadyChanged;


        UpdatePlayer();
    }

    private void CharacterSelecReady_OnReadyChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void GameNetworkConnector_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }
    private void UpdatePlayer()
    {
        if (GameNetworkConnector.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
            PlayerData playerData = GameNetworkConnector.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyTextGameObject.SetActive(CharacterSelecReady.Instance.IsPlayerReady(playerData.clientId));
            //color update
            playerVisual.SetPlayerColor(GameNetworkConnector.Instance.GetPlayerColor(playerData.colorId));
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
    private void OnDestroy()
    {
        GameNetworkConnector.Instance.OnPlayerDataNetworkListChanged -= GameNetworkConnector_OnPlayerDataNetworkListChanged;
    }
}
