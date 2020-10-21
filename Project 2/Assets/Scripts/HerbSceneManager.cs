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
    public GameObject ingredient;
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

    // float values
    private float xBounds = 0.0f;
    private float stage1Time = 7.0f;
    private float stage2Time = 10.0f;
    private float timeShown = 0.0f;

    // int values
    private int counter = 0;

    // boolean values
    private bool isCalculated = false;
    private bool startGame = false;

    // Start is called before the first frame update
    void Start()
    {
        // The X bounds of the sprite
        xBounds = ingredient.GetComponent<SpriteRenderer>().bounds.size.x;

        // Reset timer
        timer = 0;

        // Centers ingredient
        ingredient.transform.position = new Vector3(0, 0, 0);

        // Creates ingredient
        ingredient = Instantiate(ingredient, ingredient.transform.position, Quaternion.Euler(0, 0, 0));

        // Creates all of the lines
        for (int i = 0; i < 4; i++)
        {
            gameCutSpots.Add(Instantiate(line2, new Vector3((float)((random.NextDouble() * xBounds)) - (xBounds / 2), 0, -1), Quaternion.Euler(0, 0, 0)));
            playerCutSpots.Add(Instantiate(line1, new Vector3(20, 0, -1), Quaternion.Euler(0, 0, 0)));
            playerCutSpots[i].SetActive(false);

            // Checks the game cuts line positons
            CheckPositions();
        }

        ingredient.SetActive(false);    // Make the ingredient invisible
        ToggleVisiblity();              // Toggles the lines visibility

        // Sets up audio
        audio = GetComponent<AudioSource>();
        audio.volume = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // After the game starts the first stage will disable and enable certain text boxes and toggle visibilities
        if (timer > 0.0f && timer < stage1Time && !gameCutSpots[0].active)
        {
            timeShown = stage1Time;             // Time shown text set to stage 1 time
            timeLeft.enabled = true;            // Enable the time left text
            onBoardingText.enabled = false;     // Disable onboarding text

            ingredient.SetActive(true);         // Make the ingredient visible
            ToggleVisiblity();                  // Toggles the lines visibility
        }
        if (timer > stage1Time && counter < playerCutSpots.Count && timer < stage1Time + stage2Time)
        {
            // first update only
            if (gameCutSpots[0].active)
            {
                timeShown = stage2Time;     // New time shown
                ToggleVisiblity();          // Toggles the lines visibility
            }

            // Sets current player line to visible
            playerCutSpots[counter].SetActive(true);

            // Sets the lines position to the mouses positon
            Vector3 temp = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 0, -1));
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
        else if (timer > stage1Time + stage2Time && !isCalculated)
        {
            // Reverts the last line if the player didnt click 4 times
            if (counter < 3)
            {
                playerCutSpots[counter].transform.position = new Vector3(20, 0, -1);
                playerCutSpots[counter].SetActive(false);
            }

            isCalculated = true;

            // Calculates the grade
            scoreText.enabled = true;
            scoreText.text = string.Format("Score: {0}%", (int)CalculateGrade());

            ToggleVisiblity();      // Toggles the lines visibility

            // Stops the game
            startGame = false;
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
            timer += Time.deltaTime;
            timeShown = timeShown - Time.deltaTime;

            if (timeShown < 0)
                timeShown = 0;

            timeLeft.text = string.Format("Time Left: {0:F1}", timeShown);
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
        float finalGrade = 100;     // Final grade of the mini game
        float gradeDifference = 0;  // The sum of the distances
        float playerPosSum = 0;     // The players lines positions added together
        float gamePosSum = 0;       // The games lines positions added together

        // Adds up all the positons for the game and player
        for (int i = 0; i < playerCutSpots.Count; i++)
        {
            playerPosSum += playerCutSpots[i].transform.position.x + (xBounds / 2);
            gamePosSum += gameCutSpots[i].transform.position.x + (xBounds / 2);
        }

        // Calculates the difference
        gradeDifference = Mathf.Abs(playerPosSum - gamePosSum);

        // If the distance is greater then the minimum amout to get 100%
        // Calculates the grade to subtract assuming at least line was clicked
        if (gradeDifference > 0.5f && counter != 0)
        {
            finalGrade = finalGrade - ((100 / (-3 * gradeDifference + 0.5f)) + (25 * counter));
        }

        // Subtracts any missed lines from the total
        finalGrade = finalGrade - ((100 / counter) * (4 - counter));

        // Anything that exceeds the min and max will be capped
        if (finalGrade > 100)
            finalGrade = 100;
        if (finalGrade < 0)
            finalGrade = 0;

        return finalGrade;
    }
}
