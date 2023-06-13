using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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
    private NetworkVariable<State> currState = new NetworkVariable<State>(State.Idle);
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<float> cookingTimer = new NetworkVariable<float>(0f);
    private CookingSO cookingSO;
    private OverCookingSO overCookingSO;

    public override void OnNetworkSpawn()
    {
        cookingTimer.OnValueChanged += CookingTimer__OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer__OnValueChanged;
        currState.OnValueChanged += State_OnValueChanged;
    }
    private void CookingTimer__OnValueChanged(float prevValue, float newValue)
    {
        //sets the cookingTimerMax variable to the maximum cooking time specified by cookingSO.cookingTimeMax, 
        //or to a default value of 1f if cookingSO is null.
        float cookingTimerMax = cookingSO != null ? cookingSO.cookingTimeMax : 1f;

        // event that shows how much need to be in the bar
        OnProgressChanged?.Invoke(this, new I_HasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = cookingTimer.Value / cookingTimerMax
        });
    }
    private void BurningTimer__OnValueChanged(float prevValue, float newValue)
    {
        //sets the cookingTimerMax variable to the maximum cooking time specified by cookingSO.cookingTimeMax, 
        //or to a default value of 1f if cookingSO is null.
        float burningTimerMax = overCookingSO != null ? overCookingSO.burningTimeMax : 1f;

        // event that shows how much need to be in the bar
        OnProgressChanged?.Invoke(this, new I_HasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = burningTimer.Value / burningTimerMax
        });
    }
    private void State_OnValueChanged(State prevState, State newState)
    {
        //writing down event so know when to activate red part of cooking proccess
        OnStateCheck?.Invoke(this, new OnStateCheckEventArgs
        {
            state = currState.Value
        });

        if (currState.Value == State.Idle || currState.Value == State.Burned)
        {
            // and now we make timer = 0. Hide the bar
            OnProgressChanged?.Invoke(this, new I_HasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = 0f
            });
        }
    }
    //timer will ubdate every frame
    private void Update()
    {
        if (!IsServer) { return; }

        if (HasProp())
        {
            switch (currState.Value)
            {
                case State.Idle:
                    break;

                case State.Cooking:
                    cookingTimer.Value += Time.deltaTime;




                    //if it is biger than max than it coocked
                    if (cookingTimer.Value > cookingSO.cookingTimeMax)
                    {
                        //cause it fried we Despawn this version of patty and create +cooked version
                        Props.DespawnProp(GetProp());

                        Props.SpawnProp(cookingSO.output, this);


                        //coocking is finished so we put patty to cooked state
                        currState.Value = State.Cooked;
                        burningTimer.Value = 0f;
                        SetOverCookingClientRpc(GameNetworkConnector.Instance.GetPropSONumb(GetProp().GetPropsSort()));
                    }
                    break;

                case State.Cooked:
                    burningTimer.Value += Time.deltaTime;

                    //if it is biger than max than it coocked
                    if (burningTimer.Value > overCookingSO.burningTimeMax)
                    {
                        //cause it fried we Despawn this version of patty and create +cooked version
                        Props.DespawnProp(GetProp());

                        Props.SpawnProp(overCookingSO.output, this);

                        //coocking is finished so we put patty to cooked state
                        currState.Value = State.Burned;
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
                {//so we cant drop it in cookingCupboard. Non recipe object will stay in hands
                    Props prop = player.GetProp();
                    prop.SetPropParent(this);

                    InteractPickUpServerRpc(GameNetworkConnector.Instance.GetPropSONumb(prop.GetPropsSort()));

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
            SetStateIdleServerRpc();
        }
        //and if the player holding plate we despawn object and spawn it in a Plate
        //but there is no recepe for overcooked 1
        else if (player.GetProp().TryGetPlate(out Plate plate))
        {
            if (plate.TryAddProp(GetProp().GetPropsSort()))
            {
                Props.DespawnProp(GetProp());
                //and when player pick ups the object state is idle
                SetStateIdleServerRpc();
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        currState.Value = State.Idle;
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractPickUpServerRpc(int propSONumb)
    {
        //and to prevent bugs we reset Timer
        cookingTimer.Value = 0f;

        //player dropped smth so we put in cooking state
        currState.Value = State.Cooking;

        SetCookingClientRpc(propSONumb);
    }
    [ClientRpc]
    private void SetCookingClientRpc(int propSONumb)
    {
        PropsSort propSO = GameNetworkConnector.Instance.GetPropsSortFromNumb(propSONumb);
        //changing this variable so we would understand what Prop is there
        cookingSO = GetCookingSO(propSO);
    }
    [ClientRpc]
    private void SetOverCookingClientRpc(int propSONumb)
    {
        PropsSort propSO = GameNetworkConnector.Instance.GetPropsSortFromNumb(propSONumb);
        overCookingSO = GetOverCookingSO(propSO);
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
