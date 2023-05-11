using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image barImage;
    [SerializeField] private GameObject hasProgressGameObject;

    //this created cause unity dont show [SerializeField] interfaces
    private I_HasProgress hasProgress;

    // Start is called before the first frame update
    private void Start()
    {
        //progress bar
        hasProgress = hasProgressGameObject.GetComponent<I_HasProgress>();

        hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;
        //so the bar will alwayst start with 0 progress
        barImage.fillAmount = 0;
        //dont show the bar if noone interacting with it
        gameObject.SetActive(false);
    }

    // Define a method to handle changes to the Cupboard's progress
    private void HasProgress_OnProgressChanged(object sender, I_HasProgress.OnProgressChangedEventArgs p)
    {
        // Set the fill amount of the progress bar to the normalized progress value
        barImage.fillAmount = p.progressNormalized;
        //if bar is full or zero dont show it. Else show the bar
        if (p.progressNormalized == 0f || p.progressNormalized == 1f)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
