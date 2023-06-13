using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateUiLock : MonoBehaviour
{

    private enum Mode
    {
        LookAt,
        LookAtInverted,
    }

    [SerializeField] private Mode mode;

    private void LateUpdate()
    {
        switch (mode)
        {

            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;

            case Mode.LookAtInverted:
                Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position + dirFromCamera);
                break;
        }
    }
}




