using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this needs so we could make all unique stuffs 
//rmb -> create -> CutingSO -> Insert prefabs Tomato and slices etc
[CreateAssetMenu()]
public class CuttingSO : ScriptableObject
{
    //Eg 1.-receiverd tomato 2-tomato slices 3-how many cuts (3)
    public PropsSort input;
    public PropsSort output;
    public int cuttingProgressMax;
}
