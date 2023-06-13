using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateCupboard : InheritCupboard
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateDespawned;
    [SerializeField] private PropsSort plateSO;

    //timer 1 counting, second for understand when to spawn
    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 3f;

    //numb of plate spawned
    //and max number of possible stack
    private int platesSpawned;
    private int platesSpawnedMax = 3;

    private void Update()
    {
        if (!IsServer) { return; }

        //if time has passed we spawn plate
        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            //reset timer so we spawn every 3 seconds andnot every frame after 3 seconds
            spawnPlateTimer = 0f;

            if (platesSpawned < platesSpawnedMax)
            {
                SpawnPlateServerRpc();
            }

        }
    }
    [ServerRpc]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
    }
    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        platesSpawned++;

        //Invoke event. Empty = well, it happened...
        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }




    public override void InteractCupboard(Player player)
    {
        //check is there something player holding
        if (!player.HasProp())
        {
            //check is there any plates here
            if (platesSpawned > 0)
            {
                //spawn plateSO and parent - player
                Props.SpawnProp(plateSO, player);

                //We take one
                //When player pick it up we remove 1 visual          
                InteractServerRpc();
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractServerRpc()
    {
        InteractClientRpc();
    }
    [ClientRpc]
    private void InteractClientRpc()
    {
        //We take one
        //When player pick it up we remove 1 visual
        platesSpawned--;
        OnPlateDespawned?.Invoke(this, EventArgs.Empty);
    }
}
