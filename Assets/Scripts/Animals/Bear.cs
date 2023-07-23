using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : Animal
{

    private void Start()
    {
        effect = None;
    }

    int None(Animal other, int damage)
    {
        return 0;
    }
}
