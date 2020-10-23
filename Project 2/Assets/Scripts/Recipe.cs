using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Colors { Green, Red, Blue, Yellow, Orange, Purple }

public class Recipe : MonoBehaviour
{
    public Color[] colors; //Make sure there are three
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

    public void ResetIngredients()
    {
        foreach (Ingredient i in fullIngredientList)
        {
            i.GetComponent<SpriteRenderer>().sprite = i.startImage;
            i.percentageGrade = 0f;
        }
    }

    public void FillRecipe()
    {
        ResetIngredients();
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
                    colors[c] = Color.green;
                    break;
                case 1:
                    colors[c] = Color.red;
                    break;
                case 2:
                    colors[c] = Color.blue;
                    break;
                case 3:
                    colors[c] = Color.yellow;
                    break;
                case 4:
                    colors[c] = new Color(1.0f, 0.64f, 0.0f); //orange
                    break;
                case 5:
					colors[c] = new Color(0.5f, 0.0f, 0.5f); //purple
                    break;
                default:
                    break;
            }
            Ingredient temp = new Ingredient();
            temp = fullIngredientList[5];
            ingredients[c] = temp;
        }
    }
}