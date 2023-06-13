using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//so in network, parenting does not really work so
//here is solution for it
public class FollowTransform : MonoBehaviour
{
    private Transform targetTransform;

    public void SetTargetTransform(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    private void LateUpdate()
    {
        if (targetTransform == null) { return; }

        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }
}
