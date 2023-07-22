using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;
using UnityEngine.Rendering;

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
    public string name1;

    [SerializeField]
    public string name2;

    [SerializeField]
    public TextMeshProUGUI HPText;

    [SerializeField]
    TextMeshProUGUI attackText;

    [SerializeField]
    public TextMeshProUGUI cooldownText;

    [SerializeField]
    Material mat;



    //---------------------------------------------------


    public Func<Animal, int, int> effect;

    public bool combined;

    public int cooldown;

    public bool ourTeam = true;

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
        Sprite s = GetComponent<SpriteRenderer>().sprite;
        mat = GetComponent<SpriteRenderer>().material;
        mat.SetTexture("_Animal1", s.texture);
        mat.SetTexture("_Animal2", s.texture);
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
        StartCoroutine(attackFlash());
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
        else
        {
            StartCoroutine(hitFlash());
        }
    }


    public void move(Vector2Int pos)
    {
        moveEffect();
        desiredPos = pos;
        StartCoroutine(moveToPos());
    }





    //---------------------------------------------------

    public void battleOver()
    {
        HP = maxHP;
        cooldown = attackFrequencey;
        HPText.text = HP.ToString();
        attackText.text = damage.ToString();
        cooldownText.text = cooldown.ToString();
    }


    void faint()
    {
        dieEffect();
        gameObject.SetActive(false);
    }


    //---------------------------------------------------

    IEnumerator moveToPos()
    {
        yield return new WaitUntil(moving);
    }

    IEnumerator hitFlash()
    {
        mat.SetInt("_Red",1);
        yield return new WaitForSeconds(.25f);
        mat.SetInt("_Red", 0);
    }

    IEnumerator attackFlash()
    {
        mat.SetInt("_White", 1);
        yield return new WaitForSeconds(.25f);
        mat.SetInt("_White", 0);
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
            if (game.hasAnimal(Game.mousePos))
            {
                Animal a = game.animals[Game.mousePos.x][Game.mousePos.y];
                if (a.name1 + a.name2 == name1 + name2 && Game.mousePos != startPos)
                {
                    game.sell(originalPos);
                    a.damage += 1;
                    a.maxHP += 1;
                    a.HP += 1;
                    a.HPText.text = a.HP.ToString();
                    a.attackText.text = a.damage.ToString();
                    Destroy(gameObject);
                    return;
                }
                else if (!(a.combined || combined) && Game.mousePos != startPos)
                {
                    game.sell(originalPos);
                    a.combined = true;
                    a.damage = (a.damage + damage)/2;
                    a.maxHP = (maxHP + a.maxHP)/2;
                    a.HP = a.maxHP;
                    a.range = (int)Mathf.Floor((a.range+range)/2f);
                    a.attackFrequencey = (int)Mathf.Ceil((a.cooldown + cooldown) / 2f);
                    a.cooldown = a.attackFrequencey;
                    a.name2 = name2;
                    a.HPText.text = a.HP.ToString();
                    a.attackText.text = a.damage.ToString();
                    a.cooldownText.text = a.cooldown.ToString();
                    a.abilityText2 = abilityText2;
                    a.effect = effect;
                    a.mat.SetTexture("_Animal2", GetComponent<SpriteRenderer>().sprite.texture);
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    transform.position = originalPos;
                    return;
                }
            }
            transform.position = new Vector3(Game.mousePos.x, Game.mousePos.y, 0);
            startPos = Vector2Int.RoundToInt( transform.position);
            game.move(originalPos,Vector3Int.RoundToInt( transform.position));
        }
        else if (Game.mousePos.x >= 4 && Game.mousePos.x <= 6 && Game.mousePos.y >= 0 && Game.mousePos.y <= 3)
        {
            game.sell(originalPos);
            Destroy(gameObject);
        }
        else
        {
            transform.position = originalPos;
        }
    }
    [SerializeField]
    public GameObject description;
    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    TextMeshProUGUI abilityText;
    private void OnMouseEnter()
    {
        description.SetActive(true);
        nameText.text = name1 + name2;
        abilityText.text = abilityText1 + ": " + abilityText2 + "\n" + "HP: " + maxHP.ToString() + " Attack: " + damage.ToString() + " Cooldown: " + attackFrequencey.ToString() + " Range: " + getRange().ToString();
    }

    private void OnMouseExit()
    {
        description.SetActive(false);
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
