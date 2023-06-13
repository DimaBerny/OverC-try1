using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightCupboard : MonoBehaviour
{
    //for insert prefabs and visual
    [SerializeField] private InheritCupboard basicCupboard; //prefab of cupboard visual
    [SerializeField] private GameObject[] highlightObjectArray; //prefab of transparent visual
    //adding array so it could possible to process 1+ quntity of object`s visuals

    private void Start()
    {
        //if there is spawned player on the start
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnHighlightCupboard += Instance_OnHighlightCupboard;
        }
        else
        {
            Player.OnPlayerSpawned += Player_OnPlayerSpawned;
        }
    }

    private void Player_OnPlayerSpawned(object sender, EventArgs e)
    {
        //we unsub and then sub to event cause sometimes it causes errors or bugs
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnHighlightCupboard -= Instance_OnHighlightCupboard;
            Player.LocalInstance.OnHighlightCupboard += Instance_OnHighlightCupboard;
        }
    }

    private void Instance_OnHighlightCupboard(object sender, Player.OnHighlightCupboardEventArgs p)
    {
        if (p.selectedCupboard == basicCupboard)
        {
            //highlight
            foreach (GameObject highlightObject in highlightObjectArray)
            {
                highlightObject.SetActive(true);
            }
        }
        else
        {
            //turn off highlight
            foreach (GameObject highlightObject in highlightObjectArray)
            {
                highlightObject.SetActive(false);
            }
        }
    }
}