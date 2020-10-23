using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe : MonoBehaviour
{
    public Color[] colors; //Make sure there are three
    public Ingredient[] ingredients; //Make sure there are three
    public List<Ingredient> fullIngredientList; //Every ingredient
    private int[] idsUsed;
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
        idsUsed = new int[3];
        ResetIngredients();
        colors = new Color[3];
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
        }
        for(int c = 0; c < 3; c++)
        {
            bool duplicate = true;
            Ingredient temp = new Ingredient();
            while (duplicate)
            {
                duplicate = false;
                temp = fullIngredientList[Random.Range(0, fullIngredientList.Count)];
                for(int d = 0; d < c; d++)
                {
                    if(idsUsed[d] == temp.id)
                    {
                        duplicate = true;
                    }
                }
            }
            ingredients[c] = temp;
            idsUsed[c] = temp.id;
        }
    }
}