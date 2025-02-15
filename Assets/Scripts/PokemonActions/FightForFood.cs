﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightForFood : Fight
{

    
    public override bool PostPerform()
    {
        Debug.Log(name + " PostPerform FightForFood");
        if (base.PostPerform())
        {
            beliefs.RemoveState(WorldState.Label.attackingForFood);
            return true;
        }
        return false;
    }
    public override bool PrePerform()
    {
        Debug.Log(name + " PrePerform FightForFood");
        if (base.PrePerform())
        {
            //TODO: place this in belief instead of world state
            GWorld.Instance.InterruptConsumeFood(target.GetComponent<Pokemon>().inventory.FindItemWithTag("Food"));
            return true;
        }
        return false;
    }


    public override void Reset()
    {
        base.Reset();
        
        actionName = "FightForFood";

        preConditions = new WorldState[2];
        preConditions[0] = new WorldState(WorldState.Label.attackingForFood, 0);
        preConditions[1] = new WorldState(WorldState.Label.isHungry,0);

        WorldState[] afterNew = new WorldState[afterEffects.Length+1];
        afterEffects.CopyTo(afterNew, 0);
        afterEffects = afterNew;
        afterEffects[afterEffects.Length-1] = new WorldState(WorldState.Label.hasFood,0);
    }


   
}
