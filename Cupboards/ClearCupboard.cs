using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCupboard : InheritCupboard
{
    [SerializeField] private PropsSort propSO; //so = scriptable object

    public override void InteractCupboard(Player player)
    {
        //check is there any object on a cupboard
        if (!HasProp())
        {
            //no prop found so check if player holding smth drop here
            //else do nothing
            if (player.HasProp())
            {
                //from hands to cupboard
                player.GetProp().SetPropParent(this);
            }
        }
        else if (player.HasProp())
        {
            //and if the player holding plate we despawn object and spawn it in a Plate
            if (player.GetProp().TryGetPlate(out Plate plate))
            {
                if (plate.TryAddProp(GetProp().GetPropsSort()))
                {
                    Props.DespawnProp(GetProp());
                }

            }
            //not defyining Plate to avoid bug of doing it twice. Cause we did it in upper if.
            else if (GetProp().TryGetPlate(out plate))
            {
                if (plate.TryAddProp(player.GetProp().GetPropsSort()))
                {
                    Props.DespawnProp(player.GetProp());
                }

            }
        }

        //so there is smth in cupboard. 
        //player empty handed PICK IT UP!      
        else { GetProp().SetPropParent(player); }
    }
}
