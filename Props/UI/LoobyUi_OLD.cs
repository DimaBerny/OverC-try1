using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LoobyUi_Old : MonoBehaviour
{
    [SerializeField] private Button createLobby;
    [SerializeField] private Button joinLobby;
    private void Awake()
    {
        createLobby.onClick.AddListener(() =>
        {
            GameNetworkConnector.Instance.StartHost();
            LoadListOrder.LoadNetwork(LoadListOrder.Scene.SelectYourCharacter);
        });

        joinLobby.onClick.AddListener(() =>
        {
            GameNetworkConnector.Instance.StartClient();
            Hide();
        });
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
