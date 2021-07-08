﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightForFood : GAction
{
    GameObject attackTarget;
    Pokemon p;
    public override bool PostPerform()
    {
        GameObject f = inventory.FindItemWithTag("Food");
        p = GetComponent<Pokemon>();
        if (p.IsWinning(attackTarget.GetComponent<Pokemon>()))
        {
            // Give up our food and stun opponent
            attackTarget.GetComponent<GAgent>().inventory.RemoveItem(f);
            gameObject.GetComponent<GAgent>().inventory.AddItem(f);
            GWorld.Instance.PokemonFighting2Free(gameObject);
        }
        else
        {
            gameObject.GetComponent<Pokemon>().SetStun(true);
            GWorld.Instance.PokemonFighting2Stunned(gameObject);
        }

        beliefs.RemoveState(WorldState.Label.attacking);
        beliefs.RemoveState(WorldState.Label.isViolent);

        return true;
    }

    public override bool PrePerform()
    {
        attackTarget = inventory.FindItemWithTag("Pokemon");
        if (attackTarget == null) {
            return false;
        }
        inventory.RemoveItem(attackTarget);
        return true;
    }

    public override void Reset()
    {
        actionName = "FightForFood";
        cost = 3;
        duration = 5;
        preConditions = new WorldState[1];
        preConditions[0] = new WorldState(WorldState.Label.attacking, 0);
        afterEffects = new WorldState[2];
        afterEffects[0] = new WorldState(WorldState.Label.hasFood, 0);
        afterEffects[1] = new WorldState(WorldState.Label.isPeaceful, 0);

    }
}
