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
    public static bool paused;
    // Start is called before the first frame update
    void Start()
    {
        curPotion = 0;
        paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public static void Pause()
    {
        if (paused)
        {
            Time.timeScale = 1;
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
        }
        else
        {
            Time.timeScale = 0;
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
        }
        paused = !paused;
    }
}
