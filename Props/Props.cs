using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Props : NetworkBehaviour
{
    [SerializeField] private PropsSort propsSort;
    private I_PropParent propParent;
    private FollowTransform followTransform;

    //bug with plate so private changed to this
    protected virtual void Awake()
    {
        followTransform = GetComponent<FollowTransform>();
    }
    public PropsSort GetPropsSort()
    {
        return propsSort;
    }
    public void SetPropParent(I_PropParent propParent)
    {
        SetPropParentServerRpc(propParent.GetNetworkObject());

    }
    //need to tell server that everyone is worthy to be Parent
    [ServerRpc(RequireOwnership = false)]
    private void SetPropParentServerRpc(NetworkObjectReference propParentNetworkObjectReference)
    {
        SetPropParentClientRpc(propParentNetworkObjectReference);
    }
    [ClientRpc]
    private void SetPropParentClientRpc(NetworkObjectReference propParentNetworkObjectReference)
    {
        propParentNetworkObjectReference.TryGet(out NetworkObject propParentNetworkObject);
        I_PropParent propParent = propParentNetworkObject.GetComponent<I_PropParent>();

        // If this Props object already has a parent object,
        // remove the reference to the Props object from that parent
        if (this.propParent != null)
        {
            this.propParent.NullProp();
        }


        this.propParent = propParent;
        propParent.SetPropParent(this);




        followTransform.SetTargetTransform(propParent.GetPropTransform());
    }


    public I_PropParent GetPropParent()
    {
        return propParent;
    }

    public void ClearPropParent()
    {
        propParent.NullProp();
    }
    public void Despawn()
    {
        Destroy(gameObject);
    }



    //Try(), that will work on every picking up with plate
    public bool TryGetPlate(out Plate plate)
    {
        if (this is Plate)
        {
            plate = this as Plate;
            return true;
        }
        else
        {
            plate = null;
            return false;
        }
    }
    //spawner HERE to prevent code duplication
    public static void SpawnProp(PropsSort propSO, I_PropParent propParent)
    {
        GameNetworkConnector.Instance.SpawnProp(propSO, propParent);
    }
    //kill parents connection and despawn 
    public static void DespawnProp(Props prop)
    {
        GameNetworkConnector.Instance.DespawnProp(prop);
    }


}
