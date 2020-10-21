using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverallManager : MonoBehaviour
{
    public Recipe recipe;
    public float potionScore; //The final score for the current potion
    public float finalScore; //The finals score after all the potions
    public int curPotion; //0-2
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

	public enum MiniGameState {None, DustGame, LightGame, LiquidGame, MixingGame, SolidGame, PlantGame}
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
	void Awake()
    {
        scores = new float[10];
        curPotion = 0;
		paused = false;
        StartGame();
	}

    void StartGame()
    {
        curMinigame = 0;
        recipe.FillRecipe();
        playMinigame(recipe.ingredients[curMinigame]);
    }

    void EndPotion()
    {
        for(int c = 0; c < 3; c++)
        {
            potionScore += recipe.ingredients[c].percentageGrade;
        }
        potionScore /= 4;
        scores[curPotion] = potionScore;
        curPotion++;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
		//checkGameState();

	}

    public static void Pause()
    {
        if (paused)
        {
            Time.timeScale = 1;
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
			GameObject.Find("UI").GetComponent<Canvas>().enabled = true;
        }
        else
        {
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

	void playMinigame(Ingredient ingredient = null) {
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
            Destroy(activeMinigame.GetChild(c));
        }
		switch (state) {
			case MiniGameState.LightGame:
				//Debug.Log(1);
				trackerArrow.rectTransform.anchoredPosition = new Vector3(trackerArrow.rectTransform.anchoredPosition.x, 300, 0);
                Instantiate(lightMinigame, Vector3.zero, Quaternion.identity, activeMinigame);
				break;
			case MiniGameState.DustGame:
				//Debug.Log(2);
				trackerArrow.rectTransform.anchoredPosition = new Vector3(trackerArrow.rectTransform.anchoredPosition.x, 0, 0);
                Instantiate(dustMinigame, Vector3.zero, Quaternion.identity, activeMinigame);
                break;
			case MiniGameState.LiquidGame:
				//Debug.Log(3);
				trackerArrow.rectTransform.anchoredPosition = new Vector3(trackerArrow.rectTransform.anchoredPosition.x, -300, 0);
                Instantiate(liquidMinigame, Vector3.zero, Quaternion.identity, activeMinigame);
                break;
            case MiniGameState.SolidGame:
                //Debug.Log(3);
                trackerArrow.rectTransform.anchoredPosition = new Vector3(trackerArrow.rectTransform.anchoredPosition.x, -300, 0);
                Instantiate(solidMinigame, Vector3.zero, Quaternion.identity, activeMinigame);
                break;
            case MiniGameState.PlantGame:
                //Debug.Log(3);
                trackerArrow.rectTransform.anchoredPosition = new Vector3(trackerArrow.rectTransform.anchoredPosition.x, -300, 0);
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
	}

	//void getRandomIngredients()
	//{
	//	ingredients[0] = (Ingredients)Random.Range(0, 25);
	//	ingredients[1] = (Ingredients)Random.Range(0, 25);
	//	ingredients[2] = (Ingredients)Random.Range(0, 25);

	//}
}
