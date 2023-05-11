using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingVisual : MonoBehaviour
{
    [SerializeField] private CookingCupboard cookingCupboard;
    [SerializeField] private GameObject redHighlight;

    // Start is called before the first frame update
    private void Start()
    {
        // Register the CookingCupboard_OnStateCheck method as an event handler for the OnStateCheck event
        cookingCupboard.OnStateCheck += CookingCupboard_OnStateCheck;
        // Deactivate the red highlight at the beginning of the game
        redHighlight.SetActive(false);
    }

    // Define a method to handle the OnStateCheck event
    private void CookingCupboard_OnStateCheck(object sender, CookingCupboard.OnStateCheckEventArgs p)
    {
        // Determine whether to activate the red highlight based on the state of the cooking cupboard
        bool activeRed = p.state == CookingCupboard.State.Cooking || p.state == CookingCupboard.State.Cooked;
        redHighlight.SetActive(activeRed);
    }
}
