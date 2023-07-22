using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class Animal : MonoBehaviour
{

    [SerializeField]
    public int damage;
    [SerializeField]
    int range;

    [SerializeField]
    float maxSpeedAnimation;

    [SerializeField]
    public int attackFrequencey;

    [SerializeField]
    public int maxHP;

    [SerializeField]
    public string abilityText1;

    [SerializeField]
    public string abilityText2;

    [SerializeField]
    public string name;

    [SerializeField]
    TextMeshProUGUI HPText;

    [SerializeField]
    TextMeshProUGUI attackText;

    [SerializeField]
    TextMeshProUGUI cooldownText;
    


    //---------------------------------------------------


    public Func<Animal> effect;

    public bool combined;

    public int cooldown;

    public bool ourTeam;

    public BattleSystem battleSystem;

    public Game game;

    public GameObject sellSprite;

    //---------------------------------------------------


    Vector2Int desiredPos;

    Vector3Int originalPos;

    public int HP;

    public Vector2Int startPos;






    //---------------------------------------------------


    private void Awake()
    {
        battleOver();
        startPos = Vector2Int.RoundToInt(transform.position);
    }

    //---------------------------------------------------

    public void setCooldown(int c)
    {
        cooldown = c;
        cooldownText.text = cooldown.ToString();
    }

    public void moveToStartPos()
    {
        HP = maxHP;
        setCooldown(attackFrequencey);
        transform.position = (Vector2)startPos;
    }

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
        HPText.text = HP.ToString();
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

    private void battleOver()
    {
        HP = maxHP;
        cooldown = attackFrequencey;
        HPText.text = HP.ToString();
        attackText.text = damage.ToString();
        cooldownText.text = cooldown.ToString();
    }


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


    private void OnMouseDown()
    {
        if (BattleSystem.battling)
        {
            return;
        }
        sellSprite.SetActive(true);
        originalPos = Vector3Int.RoundToInt( transform.position);
    }


    private void OnMouseDrag()
    {
        if (BattleSystem.battling)
        {
            sellSprite.SetActive(false);
            return;
        }
        transform.position += (Vector3)(Game.trueMousePos - Game.lastTrueMousePos);
    }
    private void OnMouseUp()
    {
        sellSprite.SetActive(false);
        if (BattleSystem.battling)
        {
            return;
        }
        if (Game.mousePos.x >= 0 && Game.mousePos.x <= 2 && Game.mousePos.y >= 0 && Game.mousePos.y <= 3)
        {
            transform.position = new Vector3(Game.mousePos.x, Game.mousePos.y, 0);
            startPos = Vector2Int.RoundToInt( transform.position);
            game.move(originalPos,Vector3Int.RoundToInt( transform.position));
        }
        else if (Game.mousePos.x >= 3 && Game.mousePos.x <= 6 && Game.mousePos.y >= 0 && Game.mousePos.y <= 3)
        {
            game.sell(originalPos);
            Destroy(gameObject);
        }
        else
        {
            transform.position = originalPos;
        }
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
