using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : InheritCupboard
{
    public override void InteractCupboard(Player player)
    {
        //if player holding something delete that
        if (player.HasProp())
        {
            player.GetProp().Despawn();
        }
    }
}
