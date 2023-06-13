using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform templates;
    [SerializeField] private Transform recipeTemplate;
    // HIde our preview tamplate, will activate when there will be a proper one
    void Awake()
    {
        recipeTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSpawned += DManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted += DManager_OnRecipeCompleted;
        UpdateVisual();
    }

    private void DManager_OnRecipeCompleted(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void DManager_OnRecipeSpawned(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        //clearing non-needed things.
        foreach (Transform child in templates)
        {
            if (child == recipeTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList())
        {
            //creation of new recipeTemplate in templates
            Transform recipeTransform = Instantiate(recipeTemplate, templates);
            recipeTransform.gameObject.SetActive(true);
            //changing text
            recipeTransform.GetComponent<DeliverMangerRecipeUI>().SetRecipeSO(recipeSO);
        }
    }
}
