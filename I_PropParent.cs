using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_PropParent
{
    //here lays setters, getters, the one which make null and boolean to understand is there prop at all.
    public Transform GetPropTransform();
    public void SetPropParent(Props prop);
    public Props GetProp();
    public bool HasProp();
    public void NullProp();
}
