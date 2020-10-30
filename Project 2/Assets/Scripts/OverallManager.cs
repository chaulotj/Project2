using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OverallManager : MonoBehaviour
{
    public Recipe recipe;
    public float potionScore; //The final score for the current potion
    public float finalScore; //The finals score after all the potions
    public int curPotion;
    public static bool paused;

    public GameObject lightMinigame;
    public GameObject dustMinigame;
    public GameObject solidMinigame;
    public GameObject liquidMinigame;
    public GameObject plantMinigame;
    public GameObject mixingMinigame;
    public Image trackerArrow;
    public Transform activeMinigame;
    public int curMinigame;
    public float[] scores;
    public float timer;
    private float timerCutoff;
    private float timerLimit;
    public Text timeText;
    public bool lightScenePlayed = false;
    public Image Game1Image, Game2Image, Game3Image;
    public Image stepArrow1, stepArrow2, stepArrow3;
    public Text scoreText;
    public float mixingScore;

    public enum MiniGameState { None, DustGame, LightGame, LiquidGame, MixingGame, SolidGame, PlantGame }
    public MiniGameState state;

    public enum Tasks { T_Wet, T_Dust, T_Light, T_Solid, T_Plant };

    public enum Ingredients
    {
        //1-5 are wet
        //6-10 are dust
        //11-15 are light
        //16-20 are solid
        //21-25 are plants
        W_Goat, W_Blood, W_Wine, W_Venom, W_Honey,
        D_Diamond, D_Gold, D_Emerald, D_Bone, D_Goat,
        L_Gnome, L_Feather, L_Wool, L_Mane, L_Wing,
        S_Mouse, S_Dragon, S_Unicorn, S_Mizagar, S_Quail,
        P_Thyme, P_Sage, P_Rosemary, P_FlyTrap, P_Dandelion
    };

    float randomIngredient;
    // Start is called before the first frame update
    void Start()
    {
        scores = new float[10];
        curPotion = 0;
        paused = false;
        StartGame();
    }

    void StartGame()
    {
        curMinigame = 0;
        timer = 0f;
        recipe.FillRecipe();
        updateUI();
        playMinigame(recipe.ingredients[curMinigame]);
        timerCutoff = 30f;
        timerLimit = 60f;
    }

    void updateUI() {
        Game1Image.sprite = recipe.ingredients[0].GetComponent<SpriteRenderer>().sprite;
        Game2Image.sprite = recipe.ingredients[1].GetComponent<SpriteRenderer>().sprite;
        Game3Image.sprite = recipe.ingredients[2].GetComponent<SpriteRenderer>().sprite;

        stepArrow1.color = recipe.colors[0];
        stepArrow2.color = recipe.colors[1];
        stepArrow3.color = recipe.colors[2];
    }

    public void EndPotion()
    {
        Time.timeScale = 1;
        for (int c = 0; c < 3; c++)
        {
            potionScore += recipe.ingredients[c].percentageGrade;
        }
        potionScore /= 3;
        potionScore *= mixingScore;
        if (timer > timerCutoff)
        {
            float temp = (timerLimit - timer) / (timerLimit - timerCutoff);
            if (temp < 0)
            {
                temp = 0;
            }
            if (temp > 1)
            {
                temp = 1;
            }
            potionScore *= temp;
        }
        scores[curPotion] = potionScore;
        curPotion++;
        scoreText.text = "Last Potion Score: " + (int)(potionScore * 100) + "%";
        potionScore = 0f;
        if (curPotion == 5)
        {
            float finalTotal = 0;

            foreach (float f in scores)
            {
                finalTotal += f;
            }
            finalTotal /= 5;
            EndSceneManager.score = finalTotal;
            SceneManager.LoadScene("EndScene");
        }
        else
        {
            StartGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
        timer += Time.deltaTime;
        timeText.text = "Time: " + (int)timer;
        //checkGameState();
        //Debug.Log(paused);
    }

    public static void Pause()
    {
        if (paused)
        {
            GameObject.Find("GameManager").GetComponent<AudioSource>().UnPause();
            Time.timeScale = 1;
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
            GameObject.Find("UI").GetComponent<Canvas>().enabled = true;
        }
        else
        {
            GameObject.Find("GameManager").GetComponent<AudioSource>().Pause();
            Time.timeScale = 0;
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
            GameObject.Find("UI").GetComponent<Canvas>().enabled = false;
        }
        paused = !paused;
    }

	//void checkGameState() {
	//	if (Input.GetKeyDown(KeyCode.Alpha1)) state = MiniGameState.Minigame1;
	//	if (Input.GetKeyDown(KeyCode.Alpha2)) state = MiniGameState.Minigame2;
	//	if (Input.GetKeyDown(KeyCode.Alpha3)) state = MiniGameState.Minigame3;
	//	if (Input.GetKeyDown(KeyCode.Alpha4)) state = MiniGameState.MixingGame;

	//}

	public void playMinigame(Ingredient ingredient = null) {
		updateUI();
		//recipe.ResetIngredients();
		if(ingredient is LiquidIngredient)
		{
			state = MiniGameState.LiquidGame;
		}
		else if (ingredient is LightIngredient)
		{
			state = MiniGameState.LightGame;
		}
		else if (ingredient is SolidIngredient)
		{
			state = MiniGameState.SolidGame;
		}
		else if (ingredient is DustIngredient)
		{
			state = MiniGameState.DustGame;
		}
		else if (ingredient is HerbIngredient)
		{
			state = MiniGameState.PlantGame;
		}
		else
		{
			state = MiniGameState.MixingGame;
		}
		for (int c = 0; c < activeMinigame.childCount; c++)
		{
			Destroy(activeMinigame.GetChild(c).gameObject);
		}

		switch (state) {
			case MiniGameState.LightGame:
				//Debug.Log(1);
				Instantiate(lightMinigame, Vector3.zero, Quaternion.identity, activeMinigame);
				break;
			case MiniGameState.DustGame:
				//Debug.Log(2);
				Instantiate(dustMinigame, Vector3.zero, Quaternion.identity, activeMinigame);
				break;
			case MiniGameState.LiquidGame:
				//Debug.Log(3);
				Instantiate(liquidMinigame, Vector3.zero, Quaternion.identity, activeMinigame);
				break;
			case MiniGameState.SolidGame:
				//Debug.Log(3);
				Instantiate(solidMinigame, Vector3.zero, Quaternion.identity, activeMinigame);
				break;
			case MiniGameState.PlantGame:
				//Debug.Log(3);
				Instantiate(plantMinigame, Vector3.zero, Quaternion.identity, activeMinigame);
				break;
			case MiniGameState.MixingGame:
				//Debug.Log(4);
				Instantiate(mixingMinigame, Vector3.zero, Quaternion.identity, activeMinigame);
				break;
			default:
				//Debug.Log(0);
				break;
		}

		updateTrackerArrow();
	}

	void updateTrackerArrow() {
		trackerArrow.enabled = true;
		switch (curMinigame)
		{
			case 0:
				trackerArrow.rectTransform.anchoredPosition = new Vector3(trackerArrow.rectTransform.anchoredPosition.x, -373, 0);
				break;
			case 1:
				trackerArrow.rectTransform.anchoredPosition = new Vector3(trackerArrow.rectTransform.anchoredPosition.x, -129, 0);
				break;
			case 2:
				trackerArrow.rectTransform.anchoredPosition = new Vector3(trackerArrow.rectTransform.anchoredPosition.x, 112, 0);
				break;
			default:
				if (state == MiniGameState.MixingGame)
				{
					trackerArrow.rectTransform.anchoredPosition = new Vector3(trackerArrow.rectTransform.anchoredPosition.x, 360, 0);

				}
				else
				{
					trackerArrow.enabled = false;
				}

				break;
		}
	}

	//void getRandomIngredients()
	//{
	//	ingredients[0] = (Ingredients)Random.Range(0, 25);
	//	ingredients[1] = (Ingredients)Random.Range(0, 25);
	//	ingredients[2] = (Ingredients)Random.Range(0, 25);

	//}
}
