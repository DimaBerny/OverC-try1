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
                player.GetProp().SetPropParent(this);
            }
        }
        //so there is smth in cupboard. If player has smth on hands do nothing
        //if player empty handed PICK IT UP!        
        else if (!player.HasProp())
        {
            //get from cupboard to player hands
            GetProp().SetPropParent(player);
        }
    }
}
