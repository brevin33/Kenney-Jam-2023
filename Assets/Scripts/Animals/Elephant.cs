using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elephant : Animal
{

    private void Start()
    {
        effect = takeOneLess;
    }

    int takeOneLess(Animal other, int damage)
    {
        HP += 1;
        HPText.text = HP.ToString();
        return 1;
    }

    public override void attackEffect(Animal other)
    {
        effect(other, -1);
    }
}
