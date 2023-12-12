using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    Grid grid;
    public float HeightReq;
    public int maxfoodcount;
    public int foodvalue;
    public int watervalue;
    private List<Food> foodContent = new List<Food>();
    public int currentfoodcount;
    float fillPercentage => (float)currentfoodcount /(float)maxfoodcount;

    private void Start()
    {
        currentfoodcount = foodContent.Count;
        grid = FindObjectOfType<Grid>();
    }

    public void AddFood()
    {
        int randnumfood = Random.Range(1,maxfoodcount);
        for(int x = 1; x<= randnumfood; x++)
        {
            foodContent.Add(new Food( foodvalue, watervalue, true ));
        }
        currentfoodcount = foodContent.Count;
    }
    public void Eat(GameObject creature)
    {
        Movement movement = creature.GetComponent<Movement>();
        if (foodContent.Count > 0)
        {
            Food food = foodContent[foodContent.Count - 1];
            movement.currentHunger += food.nuitritionValueFood;
            movement.currentThirst += food.nuitritionValueWater;
            foodContent.RemoveAt(foodContent.Count - 1);
            currentfoodcount = foodContent.Count;
            Vector3 fillVector = new Vector3(fillPercentage, fillPercentage, fillPercentage);
            if (foodContent.Count <= 0)
            {
                Destroy(gameObject);
                grid.currentTreeNum -=1;
            }
            else
            {
                transform.localScale = fillVector;
            }
        }
    }
}