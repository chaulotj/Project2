using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverallManager : MonoBehaviour
{
    public Recipe recipe;
    public float stageOneScore; //Update from stage one
    public float potionScore; //The final score for the current potion
    public float finalScore; //The finals score after all the potions
    public int curPotion; //0-2
    public static bool paused;
    public List<Ingredient> inputIngredients; //To be added externally

	public GameObject lightMinigame;
	public GameObject dustMinigame;
	public GameObject solidMinigame;
	public GameObject liquidMinigame;
	public GameObject plantMinigame;
	public GameObject mixingMinigame;


	public enum MiniGameState {None, Minigame1,Minigame2, Minigame3, MixingGame, }
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
        curPotion = 0;
        paused = false;
		lightMinigame.SetActive(false);
		dustMinigame.SetActive(false);
		liquidMinigame.SetActive(false);
		mixingMinigame.SetActive(false);
        recipe.FillRecipe();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
		checkGameState();
		playMinigame();

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

	void checkGameState() {
		if (Input.GetKeyDown(KeyCode.Alpha1)) state = MiniGameState.Minigame1;
		if (Input.GetKeyDown(KeyCode.Alpha2)) state = MiniGameState.Minigame2;
		if (Input.GetKeyDown(KeyCode.Alpha3)) state = MiniGameState.Minigame3;
		if (Input.GetKeyDown(KeyCode.Alpha4)) state = MiniGameState.MixingGame;

	}

	void playMinigame() {
		switch (state) {
			case MiniGameState.Minigame1:
				Debug.Log(1);
				lightMinigame.SetActive(true);
				dustMinigame.SetActive(false);
				liquidMinigame.SetActive(false);
				mixingMinigame.SetActive(false);
				break;
			case MiniGameState.Minigame2:
				Debug.Log(2);
				lightMinigame.SetActive(false);
				dustMinigame.SetActive(true);
				liquidMinigame.SetActive(false);
				mixingMinigame.SetActive(false);
				break;
			case MiniGameState.Minigame3:
				Debug.Log(3);
				lightMinigame.SetActive(false);
				dustMinigame.SetActive(false);
				liquidMinigame.SetActive(true);
				mixingMinigame.SetActive(false);
				break;
			case MiniGameState.MixingGame:
				Debug.Log(4);
				lightMinigame.SetActive(false);
				dustMinigame.SetActive(false);
				liquidMinigame.SetActive(false);
				mixingMinigame.SetActive(true);
				break;
			default:
				Debug.Log(0);
				lightMinigame.SetActive(false);
				dustMinigame.SetActive(false);
				liquidMinigame.SetActive(false);
				mixingMinigame.SetActive(false);
				break;
		}
	}

	void getRandomIngredients()
	{
		ingredients[0] = (Ingredients)Random.Range(0, 25);
		ingredients[1] = (Ingredients)Random.Range(0, 25);
		ingredients[2] = (Ingredients)Random.Range(0, 25);

	}
}
