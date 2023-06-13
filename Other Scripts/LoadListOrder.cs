using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LoadListOrder
{


    public enum Scene
    {
        MainMenuScene,
        GameScene,
        LobbyReady,
        SelectYourCharacter,
        BetweenLoads,
    }


    private static Scene targetScene;



    public static void Load(Scene targetScene)
    {
        LoadListOrder.targetScene = targetScene;

        SceneManager.LoadScene(Scene.BetweenLoads.ToString());
    }

    public static void LoadNetwork(Scene targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoadCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }

}