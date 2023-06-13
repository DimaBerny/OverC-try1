using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute()]
public class PropsSort : ScriptableObject
{
    public Transform prefab;
    public string propName;
    //added sprite waaaay later so we could connect Burger part icons UI to this and 
    //Canvas knew what sprite it need to display.
    public Sprite sprite;
}
