using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Props : MonoBehaviour
{
    [SerializeField] private PropsSort propsSort;
    private I_PropParent propParent;
    public PropsSort GetPropsSort()
    {
        return propsSort;
    }
    public void SetPropParent(I_PropParent propParent)
    {
        // If this Props object already has a parent object,
        // remove the reference to the Props object from that parent
        if (this.propParent != null)
        {
            this.propParent.NullProp();
        }


        this.propParent = propParent;
        propParent.SetPropParent(this);

        //when changing parents we get new topPoint.
        //Also make local coordinates 0 0 0
        transform.parent = propParent.GetPropTransform();
        transform.localPosition = Vector3.zero;
    }
    public I_PropParent GetProp()
    {
        return propParent;
    }

    //spawner HERE to prevent code duplication
    public static Props SpawnProp(PropsSort propSO, I_PropParent propParent)
    {
        Transform propTransform = Instantiate(propSO.prefab);
        Props prop = propTransform.GetComponent<Props>();
        prop.SetPropParent(propParent);
        return prop;
    }


    //kill parents connection and despawn 
    public void Despawn()
    {
        propParent.NullProp();
        Destroy(gameObject);
    }


}
