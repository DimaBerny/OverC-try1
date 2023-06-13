using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DeliveredUI : MonoBehaviour
{
    private float textSetTime = 1.5f;
    [SerializeField] private TextMeshProUGUI comletedOrder;
    [SerializeField] private TextMeshProUGUI wrongOrder;

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeCompletedRight += DM_RightRecipe;
        DeliveryManager.Instance.OnRecipeCompletedWrong += DM_WrongRecipe;
        Hide(wrongOrder);
        Hide(comletedOrder);
    }

    private void DM_WrongRecipe(object sender, EventArgs e)
    {
        Show(wrongOrder);
        StartCoroutine(HideAfterDelay(wrongOrder));
    }

    private void DM_RightRecipe(object sender, EventArgs e)
    {
        Show(comletedOrder);
        StartCoroutine(HideAfterDelay(comletedOrder));
    }

    private void Show(TextMeshProUGUI text)
    {
        textSetTime = 1f;
        text.gameObject.SetActive(true);
    }

    private void Hide(TextMeshProUGUI text)
    {
        text.gameObject.SetActive(false);
    }

    private IEnumerator HideAfterDelay(TextMeshProUGUI text)
    {
        yield return new WaitForSeconds(textSetTime - Time.deltaTime);
        Hide(text);
    }
}