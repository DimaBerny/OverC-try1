using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingCupboard : InheritCupboard, I_HasProgress
{
    //event for progressBar
    public event EventHandler<I_HasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    [SerializeField] private CuttingSO[] slicedSOArray;

    //stuff to turn red when cooking
    public event EventHandler<OnStateCheckEventArgs> OnStateCheck;
    public class OnStateCheckEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Cooking,
        Cooked,
        Burned,
    }
    [SerializeField] private CookingSO[] cookingSOArray;
    [SerializeField] private OverCookingSO[] overCookingSOArray;

    //timer moment
    private State currState;
    private float burningTimer;
    private float cookingTimer;
    private CookingSO cookingSO;
    private OverCookingSO overCookingSO;

    private void Start()
    {
        //we are always starting in Idle state
        currState = State.Idle;
    }

    //timer will ubdate every frame
    private void Update()
    {
        if (HasProp())
        {
            switch (currState)
            {
                case State.Idle:
                    break;

                case State.Cooking:
                    cookingTimer += Time.deltaTime;

                    // event that shows how much need to be in the bar
                    OnProgressChanged?.Invoke(this, new I_HasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = cookingTimer / cookingSO.cookingTimeMax
                    });

                    //if it is biger than max than it coocked
                    if (cookingTimer > cookingSO.cookingTimeMax)
                    {
                        //cause it fried we Despawn this version of patty and create +cooked version
                        GetProp().Despawn();
                        Props.SpawnProp(cookingSO.output, this);


                        //coocking is finished so we put patty to cooked state
                        currState = State.Cooked;
                        burningTimer = 0f;
                        overCookingSO = GetOverCookingSO(GetProp().GetPropsSort());

                        //writing down event so know when to activate red part of cooking proccess
                        OnStateCheck?.Invoke(this, new OnStateCheckEventArgs
                        {
                            state = currState
                        });
                    }
                    break;

                case State.Cooked:
                    burningTimer += Time.deltaTime;

                    // event that shows how much need to be in the bar
                    OnProgressChanged?.Invoke(this, new I_HasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer / overCookingSO.burningTimeMax
                    });


                    //if it is biger than max than it coocked
                    if (burningTimer > overCookingSO.burningTimeMax)
                    {
                        //cause it fried we Despawn this version of patty and create +cooked version
                        GetProp().Despawn();
                        Props.SpawnProp(overCookingSO.output, this);

                        //coocking is finished so we put patty to cooked state
                        currState = State.Burned;

                        //writing down event so know when to activate red part of cooking proccess
                        OnStateCheck?.Invoke(this, new OnStateCheckEventArgs
                        {
                            state = currState
                        });

                        // and now we make timer = 0
                        OnProgressChanged?.Invoke(this, new I_HasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                    break;

                case State.Burned:
                    break;
            }
        }

    }

    public override void InteractCupboard(Player player)
    {
        //check is there any object on a cupboard
        if (!HasProp())
        {
            //no prop found so check if player holding smth drop here
            //else do nothing
            if (player.HasProp())
            {
                //check if has recipe 
                if (HasRecipe(player.GetProp().GetPropsSort()))
                {
                    //so we cant drop it in cookingCupboard. Non recipe object will stay in hands
                    player.GetProp().SetPropParent(this);

                    //changing this variable so we would understand what Prop is there
                    cookingSO = GetCookingSO(GetProp().GetPropsSort());

                    //player dropped smth so we put in cooking state
                    //and to prevent bugs we reset Timer
                    currState = State.Cooking;
                    cookingTimer = 0f;

                    //the state here also changed so we need to rewritte it in event
                    OnStateCheck?.Invoke(this, new OnStateCheckEventArgs
                    {
                        state = currState
                    });

                    // event that shows how much need to be in the bar
                    OnProgressChanged?.Invoke(this, new I_HasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = cookingTimer / cookingSO.cookingTimeMax
                    });
                }

            }
        }
        //so there is smth in cupboard. If player has smth on hands do nothing
        //if player empty handed PICK IT UP!        
        else if (!player.HasProp())
        {
            //get from cupboard to player hands
            GetProp().SetPropParent(player);

            //and when player pick ups the object state is idle
            currState = State.Idle;

            //the state here also changed so we need to rewritte it in event
            OnStateCheck?.Invoke(this, new OnStateCheckEventArgs
            {
                state = currState
            });

            // if player take awat patty. and now we make timer = 0
            OnProgressChanged?.Invoke(this, new I_HasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = 0f
            });
        }
    }

    //to prevent errors and check if there is recipe fo that
    private bool HasRecipe(PropsSort inputPropSO)
    {
        CookingSO cookingSO = GetCookingSO(inputPropSO);
        return cookingSO != null;
    }

    private CookingSO GetCookingSO(PropsSort inputPropSO)
    {
        foreach (CookingSO cookedSO in cookingSOArray)
        {
            //checking for everyone if this prop = input.prefab From CookingSO
            //when find we return cooked or overcooked version of it
            //using  [SerializeField]
            if (cookedSO.input == inputPropSO)
            {
                return cookedSO;
            }
        }
        return null;
    }

    private OverCookingSO GetOverCookingSO(PropsSort inputPropSO)
    {
        foreach (OverCookingSO overCookedSO in overCookingSOArray)
        {
            if (overCookedSO.input == inputPropSO)
            {
                return overCookedSO;
            }
        }
        return null;
    }
}
