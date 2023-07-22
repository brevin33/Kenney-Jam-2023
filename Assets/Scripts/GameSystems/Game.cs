using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Game : MonoBehaviour
{
    [SerializeField]
    Vector2 gridSize;

    [SerializeField]
    Camera camera;

    [SerializeField]
    SpriteRenderer[] row0;
    [SerializeField]
    SpriteRenderer[] row1;
    [SerializeField]
    SpriteRenderer[] row2;
    [SerializeField]
    SpriteRenderer[] row3;
    [SerializeField]
    float boardAlpha;
    [SerializeField]
    int maxAnimals;
    [SerializeField]
    GameObject sellSprite;
    [SerializeField]
    TextMeshProUGUI unitText;
    [SerializeField]
    Shop shop;
    [SerializeField]
    GameObject shopSprites;
    [SerializeField]
    BattleSystem battleSystem;
    [SerializeField]
    GameObject wonGame;
    //---------------------------------------------------

    public static Vector2 trueMousePos;

    public static Vector2 lastTrueMousePos;

    public static Vector2Int mousePos;

    public static Vector2Int lastMousePos;

    public List<List<Animal>> animals;

    public List<Animal> playersAnimals;

    public int numAnimals;

    public int wins = 0;

    //---------------------------------------------------

    List<List<SpriteRenderer>> board;
    //---------------------------------------------------

    private void Awake()
    {
        playersAnimals = new List<Animal>();
        board = new List<List<SpriteRenderer>>();
        board.Add(row0.ToList());
        board.Add(row1.ToList());
        board.Add(row2.ToList());
        board.Add(row3.ToList());
        animals = new List<List<Animal>>();
        for (int x = 0; x < 7; x++)
        {
            animals.Add(new List<Animal>());
        }
        for (int y = 0; y < 7; y++)
        {
            animals[y].Add(null);
            animals[y].Add(null);
            animals[y].Add(null);
            animals[y].Add(null);
        }
    }

    private void Update()
    {
        updateInputs();
        if (mousePos.x >= 0 && mousePos.x <= 6 && mousePos.y >= 0 && mousePos.y <= 3)
        {
            Color color = board[mousePos.y][mousePos.x].color;
            board[mousePos.y][mousePos.x].color = new Color(color.r,color.g,color.b,1);
            StartCoroutine(changeBackAlpha());
        }
    }



    //---------------------------------------------------

    public void spawn(GameObject animal)
    {
        if (numAnimals == maxAnimals)
        {
            return;
        }
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (animals[x][y] is null)
                {
                    makeAnimal(animal,x,y);
                    return;
                }
            }
        }
    }

    public void move(Vector3Int originalPos, Vector3Int position)
    {
        animals[position.x][position.y] = animals[originalPos.x][originalPos.y];
        animals[originalPos.x][originalPos.y] = null;
    }

    public bool hasAnimal(Vector2Int pos)
    {
        return animals[pos.x][pos.y] is not null;
    }

    public void sell(Vector3Int originalPos)
    {
        numAnimals--;
        unitText.text = numAnimals.ToString() + "/" + maxAnimals.ToString();
        animals[originalPos.x][originalPos.y] = null;
    }

    public void roundOver()
    {
        wins++;
        if (wins == 6)
        {
            wonGame.SetActive(true);
        }
        shopSprites.SetActive(true);
        shop.enableButton();
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (animals[x][y] is not null)
                {
                    animals[x][y] = null;
                }
            }
        }
        for (int i = 0; i < playersAnimals.Count; i++)
        {
            Animal a = playersAnimals[i];
            animals[a.startPos.x][a.startPos.y] = a;
            a.gameObject.SetActive(true);
            a.moveToStartPos();
        }
    }

    public void pressStart()
    {
        makeAnimalEnemy(shop.getRandomAnimal(), 5, 2);
        shopSprites.SetActive(false);
        battleSystem.startBattle();
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (animals[x][y] is not null)
                {
                    battleSystem.ourAnimals.Add(new Vector2Int(x,y));
                }
            }
        }
        for (int x = 4; x < 7; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (animals[x][y] is not null)
                {
                    battleSystem.enemyAnimals.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    //---------------------------------------------------


    void makeAnimal(GameObject animal, int x, int y)
    {
        numAnimals++;
        unitText.text = numAnimals.ToString() + "/" + maxAnimals.ToString();
        GameObject a = Instantiate(animal, new Vector2(x,y),Quaternion.identity);
        animals[x][y] = a.GetComponent<Animal>();
        a.GetComponent<Animal>().game = this;
        a.GetComponent<Animal>().sellSprite = sellSprite;
        a.GetComponent<Animal>().battleSystem = battleSystem;
        a.transform.localScale = animal.transform.localScale;
        playersAnimals.Add(a.GetComponent<Animal>());
    }

    void makeAnimalEnemy(GameObject animal, int x, int y)
    {
        GameObject a = Instantiate(animal, new Vector2(x, y), Quaternion.identity);
        animals[x][y] = a.GetComponent<Animal>();
        a.GetComponent<Animal>().game = this;
        a.GetComponent<Animal>().sellSprite = sellSprite;
        a.GetComponent<Animal>().ourTeam = false;
        a.GetComponent<Animal>().battleSystem = battleSystem;
        a.transform.localScale = animal.transform.localScale;
    }

    void updateInputs()
    {
        lastMousePos = mousePos;
        lastTrueMousePos = trueMousePos;
        trueMousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos = Vector2Int.RoundToInt(trueMousePos);
    }

    //---------------------------------------------------

    IEnumerator changeBackAlpha()
    {
        yield return new WaitUntil(newMousePos);
        Color color = board[lastMousePos.y][lastMousePos.x].color;
        board[lastMousePos.y][lastMousePos.x].color = new Color(color.r, color.g, color.b, boardAlpha);
    }

    bool newMousePos()
    {
        return mousePos != lastMousePos;
    }

}
