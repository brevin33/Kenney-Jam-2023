using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [SerializeField]
    GameObject gameOverScreen;


    //---------------------------------------------------
    public static bool battling;

    //---------------------------------------------------
    public List<Vector2Int> ourAnimals;

    public List<Vector2Int> enemyAnimals;

    float turnCooldown;

    float actionCooldown;

    public List<Vector2Int> actionsToTake;

    public int actionIndex = 999;

    //---------------------------------------------------

    private void Awake()
    {
        actionsToTake = new List<Vector2Int>();
    }

    public void startBattle()
    {
        battling = true;
        actionIndex = 999;
        ourAnimals = new List<Vector2Int>();
        enemyAnimals = new List<Vector2Int>();
        actionsToTake = new List<Vector2Int>();
        turnCooldown = 0;
        actionCooldown = 0;
    }

    private void Update()
    {
        if (battling)
        {
            if (actionIndex < actionsToTake.Count)
            {
                actionCooldown += Time.deltaTime;
                if (actionCooldown > actionSpeed)
                {
                    if (game.animals[actionsToTake[actionIndex].x][actionsToTake[actionIndex].y] is not null)
                    {
                        actionCooldown = 0;
                        takeAction(actionsToTake[actionIndex], game.animals[actionsToTake[actionIndex].x][actionsToTake[actionIndex].y].ourTeam);
                    }
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
        if (enemyAnimals.Count == 0)
        {
            battling = false;
            game.roundOver();
        }
        if (ourAnimals.Count == 0)
        {
            battling = false;
            gameOverScreen.SetActive(true);
        }
    }

    public void gameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    //---------------------------------------------------


    void takeActions()
    {
        actionsToTake = new List<Vector2Int>();
        for (int i = 0; i < ourAnimals.Count; i++)
        {
            Vector2Int animal = ourAnimals[i];
            Animal us = game.animals[animal.x][animal.y];
            us.setCooldown(us.cooldown-1);
            if (us.cooldown <= 0)
            {
                actionsToTake.Add(animal);
            }
        }
        for (int i = 0; i < enemyAnimals.Count; i++)
        {
            Vector2Int animal = enemyAnimals[i];
            Animal us = game.animals[animal.x][animal.y];
            us.setCooldown(us.cooldown - 1);
            if (us.cooldown == 0)
            {
                actionsToTake.Add(animal);
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
        us.setCooldown(us.attackFrequencey);
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
            if (game.hasAnimal( new Vector2Int(animal.x, animal.y + moveDir)))
            {
                moveDir = closestEnemy.x - animal.x > 0 ? 1 : -1;
                if (game.hasAnimal(new Vector2Int(animal.x + moveDir, animal.y)) || closestEnemy.x == animal.x)
                {
                    return;
                }
                if (isOurAnimal)
                {
                    int i = ourAnimals.IndexOf(animal);
                    ourAnimals[i] = new Vector2Int(ourAnimals[i].x + moveDir, ourAnimals[i].y);
                }
                else
                {
                    int i = enemyAnimals.IndexOf(animal);
                    enemyAnimals[i] = new Vector2Int(enemyAnimals[i].x + moveDir, enemyAnimals[i].y);
                }
                game.move(Vector3Int.RoundToInt((Vector2)animal), new Vector3Int(animal.x + moveDir, animal.y, 0));
                animal.x += moveDir;
                us.move(animal);
                return;
            }
            if (isOurAnimal)
            {
                int i = ourAnimals.IndexOf(animal);
                ourAnimals[i] = new Vector2Int(animal.x, animal.y + moveDir);
            }
            else
            {
                int i = enemyAnimals.IndexOf(animal);
                enemyAnimals[i] = new Vector2Int(enemyAnimals[i].x, enemyAnimals[i].y + moveDir);
            }
            game.move(Vector3Int.RoundToInt((Vector2)animal),new Vector3Int(animal.x,animal.y + moveDir,0));
            animal.y += moveDir;
            us.move(animal);
        }
        else
        {
            int moveDir = closestEnemy.x - animal.x > 0 ? 1 : -1;
            if (game.hasAnimal(new Vector2Int(animal.x + moveDir, animal.y)))
            {
                moveDir = closestEnemy.y - animal.y > 0 ? 1 : -1;
                if (game.hasAnimal(new Vector2Int(animal.x, animal.y + moveDir)) || closestEnemy.y == animal.y)
                {
                    return;
                }
                if (isOurAnimal)
                {
                    int i = ourAnimals.IndexOf(animal);
                    ourAnimals[i] = new Vector2Int(animal.x, animal.y + moveDir);
                }
                else
                {
                    int i = enemyAnimals.IndexOf(animal);
                    enemyAnimals[i] = new Vector2Int(enemyAnimals[i].x, enemyAnimals[i].y + moveDir);
                }
                game.move(Vector3Int.RoundToInt((Vector2)animal), new Vector3Int(animal.x, animal.y + moveDir, 0));
                animal.y += moveDir;
                us.move(animal);
                return;
            }
            if (isOurAnimal)
            {
                int i = ourAnimals.IndexOf(animal);
                ourAnimals[i] = new Vector2Int(ourAnimals[i].x + moveDir, ourAnimals[i].y);
            }
            else
            {
                int i = enemyAnimals.IndexOf(animal);
                enemyAnimals[i] = new Vector2Int(enemyAnimals[i].x + moveDir, enemyAnimals[i].y);
            }
            game.move(Vector3Int.RoundToInt((Vector2)animal), new Vector3Int(animal.x + moveDir, animal.y, 0));
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
        for (int i = 0; i < ourAnimals.Count; i++)
        {
            Vector2Int enemyAnimalPos = ourAnimals[i];
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

