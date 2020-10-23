using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DustSceneManager : MonoBehaviour
{
	private DustIngredient ingredientObj;
	public float score;

	public const float MAX_CLICKS = 5.0f;
	public int clickCounter;

	public Color dustColor;
	private SpriteRenderer s_renderer;
	public ParticleSystem ps;
	public Marker marker;
	public Marker tempMarker;
	public float height;
	public float width;
	public List<Marker> points;
	public Vector3 mouseClickPos;
	private float minDistance;
	private float tempDistance;

	public AudioClip hammerDink;
	private AudioSource source;

	public Text InstructionTxt, HintTxt;

	private bool didEnd = false;


	public OverallManager manager;

	public float distance;
	// Start is called before the first frame update
	void Start()
	{
		manager = GameObject.Find("GameManager").GetComponent<OverallManager>();
		ingredientObj = Instantiate(manager.recipe.ingredients[manager.curMinigame], new Vector3(0, 0, 1), Quaternion.identity, manager.activeMinigame) as DustIngredient;
		s_renderer = ingredientObj.GetComponent<SpriteRenderer>();

		source = GetComponent<AudioSource>();
		source.volume = 1.0f;
		source.clip = hammerDink;

		height = s_renderer.bounds.size.y;
		width = s_renderer.bounds.size.x;
		minDistance = float.MaxValue;
		points = new List<Marker>();

		setUpPoints();
		var main = ps.main;
		main.startColor = dustColor;


	}

	// Update is called once per frame
	void Update()
	{
		if (!OverallManager.paused)
		{
			if (clickCounter < MAX_CLICKS && Input.GetMouseButtonDown(0))
			{
				clickCounter++;
				mouseClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				ps.transform.position = mouseClickPos;
				if (clickCounter != MAX_CLICKS) ps.Play();
				source.Play();
				minDistance = float.MaxValue;

				foreach (Marker point in points)
				{
					tempDistance = Vector2.Distance(mouseClickPos, point.transform.position);
					if (tempDistance < minDistance)
					{
						minDistance = tempDistance;
						tempMarker = point;
					}
				}
				distance = minDistance;

				//add the score
				if (minDistance < 0.2f) score += 100;
				else if (minDistance < 0.4f) score += 90;
				else if (minDistance < 0.6f) score += 80;
				else if (minDistance < 0.8f) score += 70;
				else if (minDistance < 1.0f) score += 60;
				else if (minDistance < 1.2f) score += 50;
				else score += 0;
				//score /= clickCounter;

				points.Remove(tempMarker);
				tempMarker.gameObject.SetActive(false);

			}
			if (clickCounter >= MAX_CLICKS && !didEnd) endGame();
			if (didEnd && Input.GetKeyDown(KeyCode.Space)) nextGame();
		}
	}

	void endGame()
	{
		didEnd = true;
		score /= MAX_CLICKS;
		score /= 100.0f;
		ingredientObj.percentageGrade = score;
		manager.recipe.ingredients[manager.curMinigame].percentageGrade = ingredientObj.percentageGrade;
		manager.recipe.ingredients[manager.curMinigame].GetComponent<SpriteRenderer>().sprite = manager.recipe.ingredients[manager.curMinigame].finishedImage;
		s_renderer.sprite = ingredientObj.finishedImage;
		InstructionTxt.text = "Score: " + score*100 + "%";
		HintTxt.text = "Press 'space' to continue!";
		Time.timeScale = 0;
	}

	void nextGame() {
		Time.timeScale = 1;
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

	void setUpPoints() {
		switch (ingredientObj.id) {
			case 2: // bone
				dustColor = Color.white;
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x - width/2, gameObject.transform.position.y - height/3, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x + width/4, gameObject.transform.position.y + height/4, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x - width / 4, gameObject.transform.position.y - height / 4, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x + width/3, gameObject.transform.position.y + height/2, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -1), Quaternion.identity, manager.activeMinigame));
				break;
			case 5: //diamond
				dustColor = Color.cyan;
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x - width / 4, gameObject.transform.position.y + height / 4, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - height/2, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x - width / 4, gameObject.transform.position.y - height / 4, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x + width / 2, gameObject.transform.position.y, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -1), Quaternion.identity, manager.activeMinigame));
				break;
			case 7: //emerald
				dustColor = Color.green;
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x - width / 4, gameObject.transform.position.y + height / 4, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - height/2, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x - width / 4, gameObject.transform.position.y - height / 4, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x + width / 2, gameObject.transform.position.y, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -1), Quaternion.identity, manager.activeMinigame));
				break;
			case 9: //goat
				dustColor = Color.gray;
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x - width / 4, gameObject.transform.position.y + height / 4, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x + width/2, gameObject.transform.position.y, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - height / 3, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x + width / 4, gameObject.transform.position.y - height / 4, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -1), Quaternion.identity, manager.activeMinigame));
				break;
			case 11: //gold bar
				dustColor = Color.yellow;
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x - width / 3, gameObject.transform.position.y, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x + width /2, gameObject.transform.position.y, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + height / 5, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - height / 5, -1), Quaternion.identity, manager.activeMinigame));
				points.Add(Instantiate(marker, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -1), Quaternion.identity, manager.activeMinigame));
				break;
		}
	}
}
