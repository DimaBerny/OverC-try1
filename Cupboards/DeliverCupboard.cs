using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverCupboard : InheritCupboard
{
    public override void InteractCupboard(Player player)
    {
        //check if player has prop and it is with plate
        if (player.HasProp() && player.GetProp().TryGetPlate(out Plate plate))
        {
            //get
            DeliveryManager.Instance.DeliverRecipe(plate);

            Props.DespawnProp(player.GetProp());
        }
    }
}
