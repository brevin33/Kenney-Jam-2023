using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Slots : MonoBehaviour
{
    public Animal animal;
    [SerializeField]
    public GameObject description;

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
        nameText.text = animal.name1 + animal.name2;
        abilityText.text = animal.abilityText1 + ": " + animal.abilityText2 + "\n" + "HP: " + animal.maxHP.ToString() + " Attack: " + animal.damage.ToString() + " Cooldown: " + animal.attackFrequencey.ToString() + " Range: "+ animal.getRange().ToString();
    }

    private void OnMouseExit()
    {
        description.SetActive(false);
    }

    private void OnMouseDown()
    {
        shop.buy(animalId, this);
    }
}
