using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliverMangerRecipeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeName;
    [SerializeField] private Transform iconTemplates;
    [SerializeField] private Transform icon;

    private void Awake()
    {
        icon.gameObject.SetActive(false);
    }
    //seting new name by changing text, and we we call it in DM_UI
    //and then images/sprites
    public void SetRecipeSO(RecipeSO recipeSO)
    {
        recipeName.text = recipeSO.recipeName;

        //Clean-up. love destroying children
        foreach (Transform child in iconTemplates)
        {
            if (child == icon) continue;
            Destroy(child.gameObject);
        }

        //for each Prop in a order we pick a sprite and icon = this sprite
        foreach (PropsSort propsSort in recipeSO.propsSortList)
        {
            Transform iconTransform = Instantiate(icon, iconTemplates);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<Image>().sprite = propsSort.sprite;
        }
    }
}
