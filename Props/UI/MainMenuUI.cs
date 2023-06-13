using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button play;
    [SerializeField] private Button quit;

    private void Awake()
    {
        play.onClick.AddListener(() =>
        {
            LoadListOrder.Load(LoadListOrder.Scene.LobbyReady);
        });

        quit.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        //to fix 1 bug
        Time.timeScale = 1f;
    }
}
