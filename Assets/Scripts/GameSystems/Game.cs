using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    //---------------------------------------------------



    public static Vector2Int mousePos;

    public List<List<Animal>> animals;

    public int numAnimals;

    //---------------------------------------------------

    List<List<SpriteRenderer>> board;

    Vector2Int lastMousePos;

    //---------------------------------------------------

    private void Awake()
    {
        board = new List<List<SpriteRenderer>>();
        board.Add(row0.ToList());
        board.Add(row1.ToList());
        board.Add(row2.ToList());
        board.Add(row3.ToList());
        animals = new List<List<Animal>>();
        for (int i = 0; i < 7; i++)
        {
            animals.Add(new List<Animal>());
        }
        for (int j = 0; j < 4; j++)
        {
            animals[j].Add(null);
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
                if (animals[x][y].gameObject is null)
                {
                    makeAnimal(animal,x,y);
                    return;
                }
            }
        }
    }

    //---------------------------------------------------


    void makeAnimal(GameObject animal, int x, int y)
    {
        numAnimals++;
    }

    void updateInputs()
    {
        lastMousePos = mousePos;
        mousePos = Vector2Int.RoundToInt(camera.ScreenToWorldPoint(Input.mousePosition));
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
