using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{

    [SerializeField]
    public GameObject[] Animals;

    [SerializeField]
    Slots[] Slots;

    [SerializeField]
    SpriteRenderer[] renders;

    [SerializeField]
    Button refreshButton;

    [SerializeField]
    public GameObject ShopUI;

    [SerializeField]
    int money;

    [SerializeField]
    Game game;

    [SerializeField]
    TextMeshProUGUI moneyText;

    [SerializeField]
    TextMeshProUGUI refreshText;

    //---------------------------------------------------
    public int refreshes = 2;


    //---------------------------------------------------




    private void Awake()
    {
        enableButton();
    }

    //---------------------------------------------------

    public void refreshButtonClicked()
    {
        refreshes -= 1;
        refreshText.text = refreshes.ToString() + "x";
        if (refreshes == 0)
        {
            refreshButton.interactable = false;
        }
        refresh();
    }

    public GameObject getRandomAnimal()
    {
        return Animals[Random.Range(0, Animals.Length)];
    }

    public void enableButton()
    {
        refreshButton.interactable = true;
        refresh();
        refreshes = 2;
        refreshText.text = refreshes.ToString() + "x";
        money = 3;
        moneyText.text = "$" + money.ToString();
    }

    public void refresh()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            int r = Random.Range(0, Animals.Length);
            GameObject randomAnimal = Animals[r];
            Slots[i].animal = randomAnimal.GetComponent<Animal>();
            Slots[i].animalId = r;
            Slots[i].gameObject.SetActive(true);
            renders[i].sprite = randomAnimal.GetComponent<SpriteRenderer>().sprite;
        }
    }


    public void buy(int index, Slots slot)
    {
        if (money == 0)
        {
            return;
        }
        money -= 1;
        slot.gameObject.SetActive(false);
        slot.description.SetActive(false);
        moneyText.text = "$" + money.ToString();
        game.spawn(Animals[index]);
    }

    //---------------------------------------------------

}
