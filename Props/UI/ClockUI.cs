using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockUI : MonoBehaviour
{
    [SerializeField] private Image clock;

    private void Update()
    {
        clock.fillAmount = GameManager_.Instance.GetPlayingTimerNormalized();
    }
}
