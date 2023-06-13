using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//Enhance Props 
public class Plate : Props
{
    //event for burger visual
    //it will work when we add some of the burger`s parts.(patty, onion, buns...)
    public event EventHandler<OnAddPartBurgerEventArgs> OnAddPartBurger;
    public class OnAddPartBurgerEventArgs : EventArgs
    {
        public PropsSort propsSort;
    }

    [SerializeField] private List<PropsSort> validPropSortList;
    private List<PropsSort> propsSortList;
    //to fix multiplayer bug private has changed to this
    protected override void Awake()
    {
        base.Awake();
        propsSortList = new List<PropsSort>();
    }


    public bool TryAddProp(PropsSort propsSort)
    {
        // if it is not a valid prop -> false
        if (!validPropSortList.Contains(propsSort))
        {
            return false;
        }
        // if there is already this object in a plate -> false
        if (propsSortList.Contains(propsSort))
        {
            return false;
        }//means we can put it on a plate
        else
        {
            AddPropServerRpc(GameNetworkConnector.Instance.GetPropSONumb(propsSort));
            return true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddPropServerRpc(int propSONumb)
    {
        AddPropClientRpc(propSONumb);
    }
    [ClientRpc]
    private void AddPropClientRpc(int propSONumb)
    {
        PropsSort propsSort = GameNetworkConnector.Instance.GetPropsSortFromNumb(propSONumb);

        propsSortList.Add(propsSort);
        //when we adding burger paart we invoke the event
        OnAddPartBurger?.Invoke(this, new OnAddPartBurgerEventArgs
        {
            propsSort = propsSort
        });
    }

    //this to get List of object ON a plate right now
    //using this in a ui part where we see icons
    public List<PropsSort> GetPropsSortList() => propsSortList;
}
