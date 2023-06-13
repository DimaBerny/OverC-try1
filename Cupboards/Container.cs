using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : InheritCupboard
{
    [SerializeField] private PropsSort propSO; //so = scriptable object
    public override void InteractCupboard(Player player)
    {
        //check that player can pick up only one object
        if (!player.HasProp())
        {
            //Spawn object
            Props.SpawnProp(propSO, player);
        }
    }
}
