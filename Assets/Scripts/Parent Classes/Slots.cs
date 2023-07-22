using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Slots : MonoBehaviour
{
    public Animal animal;
    [SerializeField]
    GameObject description;

    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    TextMeshProUGUI abilityText;
    [SerializeField]
    Shop shop;

    public int animalId;
    private void OnMouseEnter()
    {
        description.SetActive(true);
        nameText.text = animal.name;
        abilityText.text = animal.abilityText;
    }

    private void OnMouseExit()
    {
        description.SetActive(false);
    }

    private void OnMouseDown()
    {
        shop.buy(animalId);
    }
}
