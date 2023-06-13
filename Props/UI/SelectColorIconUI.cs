using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectColorIconUI : MonoBehaviour
{
    [SerializeField] private int colorId;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;


    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameNetworkConnector.Instance.ChangePlayerColor(colorId);
        });
    }

    private void Start()
    {
        //when something changes in a list we will update visual
        GameNetworkConnector.Instance.OnPlayerDataNetworkListChanged += GameNetworkConnector_OnPlayerDataNetworkListChanged;
        image.color = GameNetworkConnector.Instance.GetPlayerColor(colorId);
        UpdateIsSelected();
    }

    private void GameNetworkConnector_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if (GameNetworkConnector.Instance.GetPlayerData().colorId == colorId)
        {
            selectedGameObject.SetActive(true);
        }
        else
        {
            selectedGameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        GameNetworkConnector.Instance.OnPlayerDataNetworkListChanged -= GameNetworkConnector_OnPlayerDataNetworkListChanged;
    }
}
