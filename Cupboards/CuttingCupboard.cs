using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCupboard : InheritCupboard, I_HasProgress
{
    //event for progressBar
    public event EventHandler<I_HasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    [SerializeField] private CuttingSO[] slicedSOArray;

    //to cut need more than just 1 tap
    private int cuttingProgress;

    public override void InteractCutting(Player player)
    {
        //if there is an object we slice them
        //but basically delete one spawn sliced version
        //added && so if we have already sliced tomato we can't even try to slice them twice

        if (HasProp() && HasRecipe(GetProp().GetPropsSort()))
        {
            //after interact cuttingProgress raised by 1
            cuttingProgress++;
            CuttingSO cuttingSO = GetCuttingSO(GetProp().GetPropsSort());
            //progress bar event
            OnProgressChanged?.Invoke(this, new I_HasProgress.OnProgressChangedEventArgs
            {
                //changed 1 value to float cause int/int = int BUT float/int=float
                progressNormalized = (float)cuttingProgress / cuttingSO.cuttingProgressMax
            });

            //so when we clicked enough times object will be replaced with sliced version of it
            if (cuttingProgress >= cuttingSO.cuttingProgressMax)
            {
                PropsSort slicedSO = GetSlicesFromProp(GetProp().GetPropsSort());
                GetProp().Despawn();
                Props.SpawnProp(slicedSO, this);
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
                    //so we cant drop it in cutter. Non recipe object will stay in hands
                    player.GetProp().SetPropParent(this);
                    //when drop anything the cut progress = 0
                    cuttingProgress = 0;

                    //for progress bar
                    CuttingSO cuttingSO = GetCuttingSO(GetProp().GetPropsSort());
                    OnProgressChanged?.Invoke(this, new I_HasProgress.OnProgressChangedEventArgs
                    {
                        //changed 1 value to float cause int/int = int BUT float/int=float
                        progressNormalized = (float)cuttingProgress / cuttingSO.cuttingProgressMax
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
        }
    }
    //to indetify which objects turn into what slices
    private PropsSort GetSlicesFromProp(PropsSort inputPropSO)
    {
        //getCuttingSo described below
        CuttingSO cuttingSO = GetCuttingSO(inputPropSO);
        if (cuttingSO != null)
        {
            return cuttingSO.output;
        }
        return null;
    }
    //to prevent errors and check if there is recipe fo that
    private bool HasRecipe(PropsSort inputPropSO)
    {
        CuttingSO cuttingSO = GetCuttingSO(inputPropSO);
        return cuttingSO != null;
    }

    private CuttingSO GetCuttingSO(PropsSort inputPropSO)
    {
        foreach (CuttingSO slicedSO in slicedSOArray)
        {
            //checking for everyone if this prop = input.prefab From CuttingSO
            //when find we return sliced object. For all 3 recepies has been written and inserted
            //using  [SerializeField]
            if (slicedSO.input == inputPropSO)
            {
                return slicedSO;
            }
        }
        return null;
    }
}
