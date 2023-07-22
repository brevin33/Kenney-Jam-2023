using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

public class BattleSystem : MonoBehaviour
{




    //---------------------------------------------------

    [SerializeField]
    Game game;

    [SerializeField]
    float turnSpeed;

    [SerializeField]
    float actionSpeed;


    //---------------------------------------------------
    public static bool battling;

    //---------------------------------------------------
    List<Vector2Int> ourAnimals;

    List<Vector2Int> enemyAnimals;

    float turnCooldown;

    float actionCooldown;

    List<Vector2Int> actionsToTake;

    int actionIndex = 999;

    //---------------------------------------------------

    private void Update()
    {
        if (battling)
        {
            if (actionIndex < actionsToTake.Count)
            {
                actionCooldown += Time.deltaTime;
                if (actionCooldown < actionSpeed)
                {
                    actionCooldown = 0;
                    takeAction(actionsToTake[actionIndex], game.animals[actionsToTake[actionIndex].x][actionsToTake[actionIndex].y].ourTeam);
                    actionIndex += 1;
                }
                return;
            }
            turnCooldown += Time.deltaTime;
            if (turnCooldown > turnSpeed)
            {
                turnCooldown = 0;
                actionIndex = 0;
                takeActions();
            }
        }
    }


    //---------------------------------------------------

    public void animalFainted(Vector2Int animal, bool ourTeam)
    {
        if (ourTeam)
        {
            ourAnimals.Remove(animal);
        }
        else
        {
            enemyAnimals.Remove(animal);
        }
        game.animals[animal.x][animal.y] = null;
    }


    //---------------------------------------------------


    void takeActions()
    {
        actionsToTake = new List<Vector2Int>(); ;
        for (int i = 0; i < ourAnimals.Count; i++)
        {
            Vector2Int animal = ourAnimals[i];
            Animal us = game.animals[animal.x][animal.y];
            us.setCooldown(us.cooldown-1);
            if (us.cooldown == 0)
            {
                actionsToTake.Add(animal);
                us.setCooldown(us.attackFrequencey);
            }
        }
        for (int i = 0; i < enemyAnimals.Count; i++)
        {
            Vector2Int animal = ourAnimals[i];
            Animal us = game.animals[animal.x][animal.y];
            us.setCooldown(us.cooldown - 1);
            if (us.cooldown == 0)
            {
                actionsToTake.Add(animal);
                us.setCooldown(us.attackFrequencey);
            }
        }

        actionsToTake.Sort((a, b) => 1 - 2 * Random.Range(0, 1));
    }


    void takeAction(Vector2Int animal, bool isOurAnimal)
    {
        Vector2Int closestEnemy = isOurAnimal ? findClosestAnimalOur(animal) : findClosestAnimalEnemy(animal);
        Animal us = game.animals[animal.x][animal.y];
        Animal Target = game.animals[closestEnemy.x][closestEnemy.y];
        bool attacked = false;

        if (ManhattanDistance(animal,closestEnemy) <= us.getRange())
        {
            attacked = true;
            us.attack(Target);
        }
        else
        {
            move(closestEnemy, animal, us, isOurAnimal);
        }
        if(ManhattanDistance(animal, closestEnemy) <= us.getRange() && !attacked)
        {
            us.attack(Target);
        }
    }


    private void move(Vector2Int closestEnemy, Vector2Int animal, Animal us, bool isOurAnimal)
    {
        int xdist = Mathf.Abs(closestEnemy.x - animal.x);
        int ydist = Mathf.Abs(closestEnemy.y - animal.y);
        if (ydist >= xdist)
        {
            int moveDir = closestEnemy.y - animal.y > 0 ? 1 : -1;
            animal.y += moveDir;
            us.move(animal);
        }
        else
        {
            int moveDir = closestEnemy.x - animal.x > 0 ? 1 : -1;
            animal.x += moveDir;
            us.move(animal);
        }
    }

    Vector2Int findClosestAnimalOur(Vector2Int animalPos)
    {
        Vector2Int closestAnimal = new Vector2Int(999,999);
        float closesDist = 999;
        for (int i = 0; i < enemyAnimals.Count; i++)
        {
            Vector2Int enemyAnimalPos = enemyAnimals[i];
            float dist = ManhattanDistance(animalPos, enemyAnimalPos);
            if (dist < closesDist)
            {
                closestAnimal = enemyAnimalPos;
                closesDist = dist;
            }
        }
        return closestAnimal;
    }

    Vector2Int findClosestAnimalEnemy(Vector2Int animalPos)
    {
        Vector2Int closestAnimal = new Vector2Int(999, 999);
        float closesDist = 999;
        for (int i = 0; i < enemyAnimals.Count; i++)
        {
            Vector2Int enemyAnimalPos = enemyAnimals[i];
            float dist = ManhattanDistance(animalPos, enemyAnimalPos);
            if (dist < closesDist)
            {
                closestAnimal = enemyAnimalPos;
                closesDist = dist;
            }
        }
        return closestAnimal;
    }


    int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        checked
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }
}

