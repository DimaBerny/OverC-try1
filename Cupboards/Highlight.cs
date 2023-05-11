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
        Player.Instance.OnHighlightCupboard += Instance_OnHighlightCupboard;
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