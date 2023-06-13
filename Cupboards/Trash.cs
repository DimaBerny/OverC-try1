using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Trash : InheritCupboard
{
    public override void InteractCupboard(Player player)
    {
        //if player holding something delete that
        if (player.HasProp())
        {
            Props.DespawnProp(player.GetProp());
        }
    }
    // [ServerRpc(RequireOwnership = false)]
    // private void InteractServerRpc()
    // {
    //     InteractClientRpc();
    // }
    // [ClientRpc]
    // private void InteractClientRpc()
    // {
    // }
}
