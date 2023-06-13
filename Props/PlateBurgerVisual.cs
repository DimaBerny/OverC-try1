using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateBurgerVisual : MonoBehaviour
{
    //it appears without Ser-able it will not show in editor 0_0.
    //to identify what we are adding
    [Serializable]
    public struct PropsSort_GO
    {
        public PropsSort propsSort;
        public GameObject gameObject;
    }

    [SerializeField] private Plate plate;
    [SerializeField] private List<PropsSort_GO> PropsSortGOList;

    private void Start()
    {
        plate.OnAddPartBurger += Plate_OnOnAddPartBurger;
        //disable every part of burger
        foreach (PropsSort_GO propSort_GO in PropsSortGOList)
        {
            propSort_GO.gameObject.SetActive(false);
        }
    }
    private void Plate_OnOnAddPartBurger(object sender, Plate.OnAddPartBurgerEventArgs p)
    {
        //so logic is: we cycle through all(5 in this time) parts of burger and we 
        //activate the 1 we put on plate
        foreach (PropsSort_GO propSort_GO in PropsSortGOList)
        {
            if (propSort_GO.propsSort == p.propsSort)
            {
                propSort_GO.gameObject.SetActive(true);
            }
        }
    }
}
