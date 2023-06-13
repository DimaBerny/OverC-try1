using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIcons : MonoBehaviour
{
    //2 references. 1 - plate. 2 - ui, icon + bg
    [SerializeField] private Plate plate;
    [SerializeField] private Transform iconTemplate;

    //disable visual so it will not make a fuss when the plate is empty
    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    //listening to event the same as plate
    //when some new part is added
    private void Start()
    {
        plate.OnAddPartBurger += Plate_OnAddPartBurger;
    }
    //when event called we summon UbdateIcons()
    private void Plate_OnAddPartBurger(object sender, Plate.OnAddPartBurgerEventArgs p)
    {
        UpdateIcons();
    }
    //Here we Ubdate Icons -_-
    //running through all ingredients tha added and adding specific icon(sprite) for this part of burger

    private void UpdateIcons()
    {
        //Deleting children UNLESS it is IconTemplate
        //we do this cause we DO NOT want spawning every icon each time 1 object added
        foreach (Transform child in transform)
        {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject);
        }

        //running through all ingredients
        foreach (PropsSort propsSort in plate.GetPropsSortList())
        {
            //Instantiate to create new object
            //transform not null cause we DO NOT need spawn in 0 0 0 cordinats.
            //So in our case it will spawn as a child object of Plate(reference)
            Transform iconTransform = Instantiate(iconTemplate, transform);
            //and here we activate visual. We deactivate it on Awake.
            iconTransform.gameObject.SetActive(true);
            //seting correct sprite here. Tomato -> tomato, buns -> buns
            iconTransform.GetComponent<PlateBurgerIconsConnection>().SetPropsSortConnection(propsSort);
        }
    }
}
