using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Colors { Green, Red, Blue, Yellow, Orange, Purple }

public class Recipe : MonoBehaviour
{
    public Colors[] colors; //Make sure there are three
    public Ingredient[] ingredients; //Make sure there are three
    public List<Ingredient> fullIngredientList; //Every ingredient
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FillRecipe()
    {
        int[] initialList = new int[6];
        for(int c = 0; c < 6; c++)
        {
            initialList[c] = c;
        }

        // For each spot in the array, pick
        // a random item to swap into that spot.
        for (int c = 0; c < initialList.Length - 1; c++)
        {
            int d = Random.Range(c, initialList.Length);
            int tmp = initialList[c];
            initialList[c] = initialList[d];
            initialList[d] = tmp;
        }

        for(int c = 0; c < 3; c++)
        {
            switch (initialList[c])
            {
                case 0:
                    colors[c] = Colors.Green;
                    break;
                case 1:
                    colors[c] = Colors.Red;
                    break;
                case 2:
                    colors[c] = Colors.Blue;
                    break;
                case 3:
                    colors[c] = Colors.Yellow;
                    break;
                case 4:
                    colors[c] = Colors.Orange;
                    break;
                case 5:
                    colors[c] = Colors.Purple;
                    break;
                default:
                    break;
            }
            ingredients[c] = fullIngredientList[Random.Range(0, fullIngredientList.Count)];
        }

    }
}