using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

//here store all recepies
public class DeliveryManager : NetworkBehaviour
{
    //events for UI part. 
    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeCompletedRight;
    public event EventHandler OnRecipeCompletedWrong;

    //Instance to easily contact with DelManag 
    //inside the Awake() method using Instance = this; 
    //This allows other scripts to access the DeliveryManager instance using DeliveryManager.Instance. ....
    public static DeliveryManager Instance { get; private set; }
    //reference to the list with recip
    [SerializeField] private RecipeListSO recipeListSO;
    private List<RecipeSO> waitingRecipeSOList;

    //timer stuff, I think I explained it more detailed in cookingCupboard
    private float spawnRecipeTimer = 2f;//not spawning 1 frame load
    private float spawnRecipeTimerMax = 4f; // 4 second timer
    private int waitingRecipeMax = 5; //there will not be more active recepies
    private int completedOrders;

    //initializing list 
    private void Awake()
    {
        Instance = this;
        waitingRecipeSOList = new List<RecipeSO>();
    }


    private void Update()
    {
        //will run only in server so the orders are the same
        if (!IsServer) { return; }

        //when timer reaches zero we again making him4 seconds
        //and adding new random recipe
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;
            if (GameManager_.Instance.IsPlaying() && waitingRecipeSOList.Count < waitingRecipeMax)
            {
                //random pick
                //and fix problem with client RPC by creating index so we would know number of order
                int waitingRecipeSONumb = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);

                //function Results of server`s math to clients
                SpawnRecipeClientRpc(waitingRecipeSONumb);

            }
        }
    }

    //host = server + client, so to avoid double sawn we spawn in client
    //here we take things from server and spawn in every client. 
    //Server - math and visualize results in every client UI
    [ClientRpc]
    private void SpawnRecipeClientRpc(int waitingRecipeSONumb)
    {
        RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSONumb];
        waitingRecipeSOList.Add(waitingRecipeSO);
        //event. Means recipe spawned and UI(e.g.) can listen to it and add new recipeTemplate
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }


    public void DeliverRecipe(Plate plate)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            //check if this plate burger is matched the one "some1" ordered
            if (waitingRecipeSO.propsSortList.Count == plate.GetPropsSortList().Count)
            {
                //bool to check if there is a full Match (Plate X order)
                bool plateMatch = true;
                //so there is the same amount of parts
                foreach (PropsSort recipeSO in waitingRecipeSO.propsSortList)
                {
                    //runing through all the ingredients in the Recipe that has been ordered

                    bool partHas = false;
                    foreach (PropsSort plateSO in plate.GetPropsSortList())
                    {
                        //running through all parts in Plate

                        //if Plate and Order match
                        if (plateSO == recipeSO)
                        {
                            //when part has been found bool -> true and break from cycles
                            partHas = true;
                            break;
                        }
                    }
                    //check if part hasnt been found
                    if (!partHas)
                    {
                        //so Order not the same as smth on a plate
                        plateMatch = false;
                    }
                }
                //check if plate fully match with some order
                if (plateMatch)
                {
                    //so player can deliver this one
                    //doing that in server and every client

                    DeliverCorrectRecipeServerRpc(i);
                    return;
                }
            }

        }
        //nothing has been found
        //give message to all clients through server
        DeliverWrongRecipeServerRpc();
    }

    //Correct recipe sync
    //(RequireOwnership = false) means that client also can do without owning object
    //But not sure how it works but as Todd said "It just works!"
    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSONumb)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipeSONumb);
    }
    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSONumb)
    {
        //and remove this order from waiting List
        waitingRecipeSOList.RemoveAt(waitingRecipeSONumb);

        //event. UI listens and we can use it to remove it from orders
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        // here saying tha it was right order
        OnRecipeCompletedRight?.Invoke(this, EventArgs.Empty);
        completedOrders++;
    }

    //Wrong recipe sync
    [ServerRpc(RequireOwnership = false)]
    private void DeliverWrongRecipeServerRpc()
    {
        DeliverWrongRecipeClientRpc();
    }
    [ClientRpc]
    private void DeliverWrongRecipeClientRpc()
    {
        // here marking it was wrong choice
        OnRecipeCompletedWrong?.Invoke(this, EventArgs.Empty);
    }



    //getter of active orders. Using in d manager UI
    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetCompletedOrders()
    {
        return completedOrders;
    }
}
