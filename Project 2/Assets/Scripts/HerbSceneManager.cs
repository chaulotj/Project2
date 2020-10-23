using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Random = System.Random;

public class HerbSceneManager : MonoBehaviour
{
    // Public Variables
    // audio clips
    public AudioClip tick;
    public AudioClip slice;

    // game objects
    public HerbIngredient ingredient;
    public GameObject line1;
    public GameObject line2;

    // text boxes
    public Text scoreText;
    public Text onBoardingText;
    public Text timeLeft;

    // Timer
    public static float timer;

    // Private Variables
    // audio source
    private AudioSource audio;

    // List of lines
    private List<GameObject> gameCutSpots = new List<GameObject>();
    private List<GameObject> playerCutSpots = new List<GameObject>();

    // random
    private Random random = new Random();

    private OverallManager manager;

    // float values
    private float xBounds = 0.0f;
    private float stage1Time = 7.0f;
    private float stage2Time = 10.0f;
    private float timeShown = 0.0f;
    private float startTime;

    // int values
    private int counter = 0;

    // boolean values
    private bool isCalculated = false;
    private bool startGame = false;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<OverallManager>();

        // Creates ingredient
        ingredient = Instantiate(manager.recipe.ingredients[manager.curMinigame], Vector3.zero, Quaternion.Euler(0, 0, 0)) as HerbIngredient;
        ingredient.transform.parent = GameObject.Find("ActiveMinigame").transform;

        // The X bounds of the sprite
        xBounds = ingredient.GetComponent<SpriteRenderer>().bounds.size.x;

        // Reset timer
        timer = 0;

        // Centers ingredient
        ingredient.transform.position = new Vector3(0, 0, 0);

        // Creates all of the lines
        for (int i = 0; i < 4; i++)
        {
            gameCutSpots.Add(Instantiate(line2, new Vector3((float)((random.NextDouble() * xBounds)) - (xBounds / 2), 0, -1), Quaternion.Euler(0, 0, 0)));
            playerCutSpots.Add(Instantiate(line1, new Vector3(20, 0, -1), Quaternion.Euler(0, 0, 0)));
            playerCutSpots[i].SetActive(false);

            gameCutSpots[i].transform.parent = GameObject.Find("ActiveMinigame").transform;
            playerCutSpots[i].transform.parent = GameObject.Find("ActiveMinigame").transform;

            // Checks the game cuts line positons
            CheckPositions();
        }

        ingredient.gameObject.SetActive(false);     // Make the ingredient invisible
        ToggleVisiblity();                          // Toggles the lines visibility

        // Sets up audio
        audio = GetComponent<AudioSource>();
        audio.volume = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!OverallManager.paused)
        {
            // after the game starts the first stage will disable and enable certain text boxes and toggle visibilities
            if (timer > 0.0f && timer < stage1Time && !gameCutSpots[0].active)
            {
                Time.timeScale = 0;

                timeShown = stage1Time;             // Time shown text set to stage 1 time
                timeLeft.enabled = true;            // Enable the time left text
                onBoardingText.enabled = false;     // Disable onboarding text

                ingredient.gameObject.SetActive(true);     // Make the ingredient visible
                ToggleVisiblity();                  // Toggles the lines visibility
            }
            if (timer > stage1Time && counter < playerCutSpots.Count && timer < stage1Time + stage2Time)
            {

                Time.timeScale = 1;

                // first update only
                if (gameCutSpots[0].active)
                {
                    timeShown = stage2Time;     // New time shown
                    ToggleVisiblity();          // Toggles the lines visibility
                }

                // Sets current player line to visible
                playerCutSpots[counter].SetActive(true);

                // Sets the lines position to the mouses positon
                Vector3 temp = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 0, -2));
                temp.y = 0;
                temp.z = -1;

                // Keeps the lines inbounds of the sprite
                if (temp.x > xBounds / 2)
                    temp.x = xBounds / 2;
                else if (temp.x < -xBounds / 2)
                    temp.x = -xBounds / 2;

                playerCutSpots[counter].transform.position = temp;

                // If the player clicks it sets the players cut spot to the last known mouse postion
                if (Input.GetMouseButtonDown(0))
                {
                    // Playes audio clip
                    audio.clip = slice;
                    audio.Play();

                    // Next line
                    counter++;
                }
            }

            if (timer > stage1Time + stage2Time && !isCalculated || counter == 4 && !isCalculated)
            {
                int grade = (int)CalculateGrade();

                // Reverts the last line if the player didnt click 4 times
                Time.timeScale = 0;

                // Reverts the last line if the player didnt click 4 times
                if (counter < 4)
                {
                    playerCutSpots[counter].transform.position = new Vector3(20, 0, -1);
                    playerCutSpots[counter].SetActive(false);
                }

                isCalculated = true;
                // Calculates the grade
                scoreText.enabled = true;
               scoreText.text = string.Format("Score: {0}%", grade);

                    ToggleVisiblity();      // Toggles the lines visibility

                timeShown = 5;
            }

            if (timer > stage1Time + stage2Time + 5 && isCalculated)
            {

                manager.recipe.ingredients[manager.curMinigame].percentageGrade = ingredient.percentageGrade;
                manager.recipe.ingredients[manager.curMinigame].GetComponent<SpriteRenderer>().sprite = manager.recipe.ingredients[manager.curMinigame].finishedImage;
                ingredient.GetComponent<SpriteRenderer>().sprite = manager.recipe.ingredients[manager.curMinigame].finishedImage;

                manager.curMinigame++;

                if (manager.curMinigame == 3)
                {
                    manager.playMinigame();
                }
                else
                {
                    manager.playMinigame(manager.recipe.ingredients[manager.curMinigame]);
                }
            }

            if (!startGame && Input.GetMouseButtonDown(0))
            {
                // Plays audio clip
                audio.clip = tick;
                audio.Play();
                
                // Starts the game
                startGame = true;
            }
            if (startGame)
            {
                // Updates the time and time shown
                timer += Time.unscaledDeltaTime;
                timeShown = timeShown - Time.unscaledDeltaTime;

                if (timeShown < 0)
                    timeShown = 0;

                timeLeft.text = string.Format("Time Left: {0:F1}", timeShown);
            }
        }
    }

    /// <summary>
    /// Makes sure that none of the lines are overlapping
    /// </summary>
    private void CheckPositions()
    {
        for (int i = 0; i < gameCutSpots.Count; i++)
        {
            for (int j = 0; j < i; j++)
            {
                // Compares every line an checks if they are overlapping
                // If they are the function will change the position of that line and recursively do this till every line is seperated
                if (i != j && gameCutSpots[i].transform.position.x < gameCutSpots[j].transform.position.x + 0.25f &&
                    gameCutSpots[i].transform.position.x > gameCutSpots[j].transform.position.x - 0.25f)
                {
                    gameCutSpots[i].transform.position = new Vector3((float)((random.NextDouble() * xBounds)) - (xBounds / 2), 0, -1);

                    CheckPositions();
                }
            }
        }
    }

    /// <summary>
    /// Toggles the visibility of the lines
    /// </summary>
    public void ToggleVisiblity()
    {
        if (gameCutSpots[0].active)
        {
            foreach (GameObject lines in gameCutSpots)
            {
                lines.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject lines in gameCutSpots)
            {
                lines.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Calculates the final grade of the herb mini game
    /// </summary>
    /// <returns> the grade of the mini game</returns>
    private float CalculateGrade()
    {
        // Local Variables
        // floats
        float finalGrade = 100;         // Final grade of the mini game
        float gradeSum = 0;             // The sum of the distances
        float[] grades = new float[4];  // All the grades

        // integers
        int[] usedPlayerIndicies = { -1, -1, -1, -1 };  // Keep track of which indicies got used
        int[] usedGameIndicies = { -1, -1, -1, -1 };    // Keep track of which indicies got used
        int iterations = 0;                             // How many times CalculateDistance will be called

        // Calculate the distances
       CalculateDistance(usedPlayerIndicies, usedGameIndicies, grades, iterations);

        // Add the distances together
        for (int i = 0; i < grades.Length; i++)
        {
            gradeSum += grades[i];
        }

        // If the distance is greater then the minimum amout to get 100%
        // Calculates the grade to subtract assuming at least line was clicked
        if (gradeSum > 0.5f && counter != 0)
        {
            finalGrade = finalGrade - ((100 / (-3 * gradeSum + 0.5f)) + (25 * counter));
        }

        // Subtracts any missed lines from the total
        finalGrade = finalGrade - ((100 / playerCutSpots.Count) * (4 - counter));

        // Anything that exceeds the min and max will be capped
        if (finalGrade > 100)
            finalGrade = 100;
        if (finalGrade < 0)
            finalGrade = 0;

        return finalGrade;
    }

    /// <summary>
    /// Calculates the shortest (optimal) distances betweem the indicies
    /// </summary>
    /// <param name="usedPlayerIndicies">The index of the used player lines</param>
    /// <param name="usedGameIndicies">The index of the used game lines</param>
    /// <param name="grades">each grade per a line</param>
    /// <param name="iterations">number of times this method will be called</param>
    private void CalculateDistance(int[] usedPlayerIndicies, int[] usedGameIndicies, float[] grades, int iterations)
    {
        int indexP = 0;     // Index of lowest value player line
        int indexG = 0;     // Index of lowest value game line

        float lowestValue = 100;    // Tracks the lowest value

        for (int i = 0; i < playerCutSpots.Count; i++)
        {
            for (int j = 0; j < playerCutSpots.Count; j++)
            {
                // Calculated the distance
                float distance = Mathf.Abs(playerCutSpots[i].transform.position.x + (xBounds / 2) - (gameCutSpots[j].transform.position.x + (xBounds / 2)));

                // Finds the lowest value distance foe the remaining indicies
                if (distance < lowestValue &&
                    i != usedPlayerIndicies[0] && i != usedPlayerIndicies[1] && i != usedPlayerIndicies[2] && i != usedPlayerIndicies[3] &&
                    j != usedGameIndicies[0] && j != usedGameIndicies[1] && j != usedGameIndicies[2] && j != usedGameIndicies[3])
                {
                    lowestValue = distance;
                    indexP = i;
                    indexG = j;
                }
            }
        }

        // Stores Data for next iteration
        grades[iterations] = lowestValue;
        usedPlayerIndicies[iterations] = indexP;
        usedGameIndicies[iterations] = indexG;
        iterations += 1;

        // Iterates till loop = number of lines
        if (iterations < playerCutSpots.Count)
        {
            CalculateDistance(usedPlayerIndicies, usedGameIndicies, grades, iterations);
        }
    }
}
