using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InheritCupboard : NetworkBehaviour, I_PropParent
{
    [SerializeField] protected Transform topPoint; //point where things will spawn

    //Props class which contain info of PropSort where we connected cheese/onion/tomato etc
    //we need this so player understand what object is not just simple PROP but a TOMATO.
    protected Props prop;
    public virtual void InteractCupboard(Player player)
    {

    }
    public virtual void InteractCutting(Player player)
    {

    }

    public Transform GetPropTransform()
    {
        return topPoint;
    }
    public void SetPropParent(Props prop)
    {
        this.prop = prop;
    }
    public Props GetProp()
    {
        return prop;
    }
    public bool HasProp()
    {
        return prop != null;
    }
    public void NullProp()
    {
        prop = null;
    }
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
