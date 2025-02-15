﻿using System.Collections.Generic;
using UnityEngine;

public sealed class GWorld {

    // Our GWorld instance
    private static readonly GWorld instance = new GWorld();
    // Our world states
    private static WorldStates world;
    // A list of freely roaming pokemon
    private static List<GameObject> freePokemon;
    // A list of freely eating pokemon
    private static List<GameObject> eatingPokemon;
    // A list of pokemon who fight
    private static List<GameObject> fightingPokemon;
    // A list of "dead" pokemon
    private static List<GameObject> stunnedPokemon;

    // A list of freely available food
    private static List<GameObject> freeFood;
    // A list of food that is being eaten right now
    private static List<GameObject> eatenFood;
    // A list of food that is fought over
    private static List<GameObject> fightFood;


    static GWorld() {

        // Create our world
        world = new WorldStates();
        // Create pokemon lists
        freePokemon =     new List<GameObject>();
        eatingPokemon =   new List<GameObject>();
        fightingPokemon = new List<GameObject>();
        stunnedPokemon =  new List<GameObject>();

        // Create food lists
        freeFood =  new List<GameObject>();
        eatenFood = new List<GameObject>();
        fightFood = new List<GameObject>();

        // Find all GameObjects that are tagged "Food"
        GameObject[] fruits = GameObject.FindGameObjectsWithTag("Food");
        // Then add them to the cubicles Queue
        foreach (GameObject f in fruits) {

            freeFood.Add(f);
        }

        // Inform the state
        if (fruits.Length > 0) {
            world.ModifyState(WorldState.Label.availableFood, fruits.Length);
        }

        // Find all GameObjects that are tagged "Food"
        GameObject[] pokemons = GameObject.FindGameObjectsWithTag("Pokemon");
        // Then add them to the cubicles Queue
        foreach (GameObject p in pokemons)
        {

            freePokemon.Add(p);
        }

        // Inform the state
        if (pokemons.Length > 0)
        {
            world.ModifyState(WorldState.Label.availablePokemon, pokemons.Length);
        }


        // Set the time scale in Unity
        Time.timeScale = 5.0f;

        
    }

    private GWorld() {

    }

    private bool MoveObjectFromTo(GameObject obj, List<GameObject> from, List<GameObject> to)
    {
        if (from.Contains(obj))
        {
            from.Remove(obj);
            to.Add(obj);
            return true;
        }

        return false;
    }

    public void PokemonFree2Eating(GameObject p)
    {
        if(MoveObjectFromTo(p, freePokemon, eatingPokemon))
        {
            world.ModifyState(WorldState.Label.availablePokemon, -1);
            world.ModifyState(WorldState.Label.eatingPokemon, 1);
        }
            
        
    }

    public void PokemonFree2Fighting(GameObject p)
    {
        if (MoveObjectFromTo(p, freePokemon, fightingPokemon)) {
            world.ModifyState(WorldState.Label.availablePokemon, -1);
            world.ModifyState(WorldState.Label.fightingPokemon, 1);
        }
            
    }

    public void PokemonFighting2Stunned(GameObject p)
    {
        if(MoveObjectFromTo(p, fightingPokemon, stunnedPokemon)) {
            world.ModifyState(WorldState.Label.fightingPokemon, -1);
            world.ModifyState(WorldState.Label.stunnedPokemon, 1);
        };
    }

    public void PokemonFighting2Eating(GameObject p)
    {
        if(MoveObjectFromTo(p, fightingPokemon, eatingPokemon)) {
        
            world.ModifyState(WorldState.Label.fightingPokemon, -1);
            world.ModifyState(WorldState.Label.eatingPokemon,1);
        }
    }

    public void PokemonFighting2Free(GameObject p)
    {
        
        if(MoveObjectFromTo(p, fightingPokemon, freePokemon)) {
            world.ModifyState(WorldState.Label.fightingPokemon, -1);
            world.ModifyState(WorldState.Label.availablePokemon, 1);
        }
    }

    public void PokemonEating2Fighting(GameObject p)
    {
        if(MoveObjectFromTo(p, eatingPokemon, fightingPokemon)) {
            world.ModifyState(WorldState.Label.eatingPokemon, -1);
            world.ModifyState(WorldState.Label.fightingPokemon, 1);
        }
    }

    public void PokemonEating2Free(GameObject p)
    {
        if(MoveObjectFromTo(p, eatingPokemon, freePokemon)) {
            world.ModifyState(WorldState.Label.eatingPokemon, -1);
            world.ModifyState(WorldState.Label.availablePokemon,1);
        }
    }

    public void PokemonStunned2Free(GameObject p)
    {
        if(MoveObjectFromTo(p, stunnedPokemon, freePokemon)) {
            world.ModifyState(WorldState.Label.stunnedPokemon, -1);
            world.ModifyState(WorldState.Label.availablePokemon,1);
        }
    }

    // Remove Food
    public bool RemoveFood(GameObject f)
    {
        if (freeFood.Contains(f))
        {
            freeFood.Remove(f);
            return true;
        }

        else if (eatenFood.Contains(f))
        {
            eatenFood.Remove(f);
            return true;
        }

        else if (fightFood.Contains(f))
        {
            fightFood.Remove(f);
            return true;
        }
        return false;
    }

    // Add Count total food on the map
    public int FoodCounter()
    {
        int counter = 0;
        counter += (freeFood.Count + eatenFood.Count + fightFood.Count);
        return counter;
    }

    // Add Food
    public void AddNewFood(GameObject f)
    {
        freeFood.Add(f);
        world.ModifyState(WorldState.Label.availableFood, 1);
    }

    public void FoodFree2Eaten(GameObject f)
    {
        MoveObjectFromTo(f, freeFood, eatenFood);
        world.ModifyState(WorldState.Label.foodEaten, 1);
        world.ModifyState(WorldState.Label.availableFood, -1);
    }

    /*public void FoodEaten2Fight(GameObject f)
    {
        MoveObjectFromTo(f, eatenFood, fightFood);
    }*/

    /*public void FoodFight2Eaten(GameObject f)
    {
        MoveObjectFromTo(f, fightFood, eatenFood);
    }*/

    /*public void FoodFight2Free(GameObject f)
    {
        MoveObjectFromTo(f, fightFood, freeFood);
    }*/

    public void FoodEaten2Free(GameObject f)
    {
        MoveObjectFromTo(f, eatenFood, freeFood);
    }
    

    private GameObject GetClosestObject(GameObject obj, List<GameObject> ObjList, Pokemon.PokemonType ptype= Pokemon.PokemonType.NULL)
    {
        GameObject closestObj = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = obj.transform.position;

        if (ptype == Pokemon.PokemonType.NULL)
        {
            foreach (GameObject o in ObjList)
            {
                if (o == obj)
                    continue;
                float dist = Mathf.Abs(Vector3.Distance(o.transform.position, currentPos));
                if (dist < minDistance)
                {
                    closestObj = o;
                    minDistance = dist;
                }
            }
        }
        else
        {
            foreach (GameObject o in ObjList)
            {
                if (o == obj)
                    continue;
                if (o.GetComponent<Pokemon>().GetMyPokemonType() == ptype)
                {
                    float dist = Mathf.Abs(Vector3.Distance(o.transform.position, currentPos));
                    if (dist < minDistance)
                    {                
                        closestObj = o;
                        minDistance = dist;
                    }
                }
            }
        }

        return closestObj;
    }

    public Pokemon GetClosestFreePokemon(GameObject obj, Pokemon.PokemonType ptype)
    {
        GameObject closestObj = GetClosestObject(obj, freePokemon, ptype);
        
        if (closestObj != null)
        {
            return closestObj.GetComponent<Pokemon>();
        }

        return null;
    }

    public Pokemon GetClosestEatingPokemon(GameObject obj, Pokemon.PokemonType ptype)
    {
        GameObject closestObj = GetClosestObject(obj, eatingPokemon, ptype);
        if (closestObj != null)
        {
            return closestObj.GetComponent<Pokemon>();
        }

        return null;
    }

    public Food GetClosestFreeFood(GameObject obj)
    {
        GameObject closestObj = GetClosestObject(obj, freeFood);
        if (closestObj != null)
        {
            return closestObj.GetComponent<Food>();
        }

        return null;
    }

    public Food GetClosestEatenFood(GameObject obj)
    {
        GameObject closestObj = GetClosestObject(obj, eatenFood);
        if (closestObj != null)
        {
            return closestObj.GetComponent<Food>();
        }

        return null;
    }

    public int freeFoodCount() {
        return freeFood.Count;
    }

    public int eatenFoodCount() {
        return eatenFood.Count;
        
    }
    public bool ClaimFood(Food food) {
        if (food == null) return false;

        if (freeFood.Contains(food.gameObject)) {
            FoodFree2Eaten(food.gameObject);
            return true;
        }
        else return false; //food is already claimed. Pokemon sad (or fight?).
    }

    public void ConsumeFood(GameObject f) {
        if (f == null || f.GetComponent<Food>() == null) {
            Debug.Log("Trying to consume null object");
        }
        else if (eatenFood.Contains(f)) {
            eatenFood.Remove(f);
            //world.ModifyState(WorldState.Label.foodEaten, -1);
            GameObject.Destroy(f);
        }
        else Debug.Log("Trying to remove Food not currently in eatenFood");
    }

    public void InterruptConsumeFood(GameObject f)
    {
        if (f == null || f.GetComponent<Food>() == null)
        {
            Debug.Log("Trying to interrupt null object");
        }
        else if (eatenFood.Contains(f))
        {
            eatenFood.Remove(f);
            FoodEaten2Free(f.gameObject);
            GameObject.Destroy(f);
        }
        else Debug.Log("Trying to remove Food not currently in eatenFood");
    }

    public bool InitFight(Pokemon p)
    {
        if (p == null) return false;

        if (freePokemon.Contains(p.gameObject))
        {
            PokemonFree2Fighting(p.gameObject);
            return true;
        }
        else return false; //Pokemon is already eating? Should it try to init food fight?
    }

    public bool InitFoodFight(Pokemon p)
    {
        if (p == null) return false;

        if (eatingPokemon.Contains(p.gameObject))
        {
            PokemonEating2Fighting(p.gameObject);
            return true;
        }
        else return false; //Pokemon is already fighting?
    }

    public static GWorld Instance {

        get { return instance; }
    }

    public WorldStates GetWorld() {

        return world;
    }
}
