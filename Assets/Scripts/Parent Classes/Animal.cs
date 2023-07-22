using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class Animal : MonoBehaviour
{

    [SerializeField]
    int damage;
    [SerializeField]
    int range { get; }

    [SerializeField]
    float maxSpeedAnimation;

    [SerializeField]
    public float attackFrequencey;

    [SerializeField]
    int maxHP;

    [SerializeField]
    public string abilityText;

    [SerializeField]
    public string name;

    //---------------------------------------------------


    public Func<Animal> effect;

    public bool combined;

    public float cooldown;

    public bool ourTeam;

    public BattleSystem battleSystem;


    //---------------------------------------------------


    Vector2Int desiredPos;

    int HP;






    //---------------------------------------------------

    private void Update()
    {
        
    }



    //---------------------------------------------------


    public int getRange()
    {
        return range;
    }

    public void attack(Animal other)
    {
        attackEffect(other);
        other.hit(damage, other);
    }

    public void hit(int damage,Animal other)
    {
        hitEffect(other,damage);
        HP -= damage;
        if (HP <= 0)
        {
            battleSystem.animalFainted(Vector2Int.RoundToInt(transform.position), ourTeam);
            faint();
        }
    }


    public void move(Vector2Int pos)
    {
        moveEffect();
        desiredPos = pos;
        StartCoroutine(moveToPos());
    }





    //---------------------------------------------------


    void faint()
    {
        gameObject.SetActive(false);
    }


    //---------------------------------------------------

    IEnumerator moveToPos()
    {
        yield return new WaitUntil(moving);
    }

    private bool moving()
    {
        float newX = Mathf.MoveTowards(transform.position.x, desiredPos.x, maxSpeedAnimation);
        float newY = Mathf.MoveTowards(transform.position.y, desiredPos.y, maxSpeedAnimation);
        Vector2 newPos = new Vector2(newX, newY);
        transform.position = newPos;
        return newPos == desiredPos;
    }





    //---------------------------------------------------

    public virtual void attackEffect(Animal other)
    {

    }

    public virtual void hitEffect(Animal other, int damage)
    {

    }

    public virtual void dieEffect()
    {

    }

    public virtual void moveEffect()
    {

    }

    public virtual void battleStartEffect()
    {

    }

}
