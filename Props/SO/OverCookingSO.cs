using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this needs so we could make all unique stuffs 
//rmb -> create -> OverCookingSO -> Insert prefabs
[CreateAssetMenu()]
public class OverCookingSO : ScriptableObject
{
    //Give him time to cook. Btw using float not int
    public PropsSort input;
    public PropsSort output;
    public float burningTimeMax;
}
