using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverallManager : MonoBehaviour
{
    public Recipe recipe;
    public Ingredient[] ingredients; //Needs three
    public float stageOneScore; //Update from stage one
    public float potionScore; //The final score for the current potion
    public float finalScore; //The finals score after all the potions
    public int curPotion; //0-2
    // Start is called before the first frame update
    void Start()
    {
        curPotion = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
