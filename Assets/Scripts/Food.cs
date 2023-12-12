using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Food
{
    public int nuitritionValueFood;
    public int nuitritionValueWater;
    public bool isFruit;

    public Food( int foodValue, int WaterValue, bool fruit)
    {
        nuitritionValueFood = foodValue;
        nuitritionValueWater = WaterValue;
        isFruit = fruit;

    }
}
