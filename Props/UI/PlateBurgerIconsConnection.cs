using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//connected to IconTemplate btw, not burgerIcons

public class PlateBurgerIconsConnection : MonoBehaviour
{
    //here we gram image of the icon and set proper one from PropsSort sprites
    [SerializeField] private Image image;

    public void SetPropsSortConnection(PropsSort propsSort)
    {
        image.sprite = propsSort.sprite;
    }
    //also here we could add some animation things, that every time icons appeared
    // we could add patrticles or smth, but there is no time for that
}
