using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private MeshRenderer bodyMeshRenderer;

    private Material material;

    //every player can pick unique color without changing all team
    private void Awake()
    {
        //creation of material and setting it
        material = new Material(bodyMeshRenderer.material);
        bodyMeshRenderer.material = material;
    }


    public void SetPlayerColor(Color color)
    {
        material.color = color;
    }
}
