using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MixingSceneManager : MonoBehaviour
{
    private float score;
    public OverallManager manager;
    private Ingredient[] ingredientObjs;
    private int ingredientsAdded;
    public GameObject cauldron;
    private GameObject cauldronObj;
    public GameObject otherCauldron;
    public GameObject ladle;
    private GameObject ladleObj;
    private bool carryingIngredient;
    private Ingredient carriedIngredient;
    private float stirringDoneAmount;
    private Vector2 lastPos;
    public Text scoreText;
    public AudioClip tick;
    public AudioClip splash;
    private AudioSource source;
    private bool timing;
    private float revealTimer;
    float potionScoreAmount;
    private bool canClickDone;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<OverallManager>();
        ingredientsAdded = 0;
        carryingIngredient = false;
        score = 1f;
        ingredientObjs = new Ingredient[3];
        cauldronObj = Instantiate(cauldron, new Vector3(0.0f, -1.8f, 0.0f), Quaternion.identity, manager.activeMinigame) as GameObject;
        cauldronObj.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        ladleObj = Instantiate(ladle, Vector3.zero, Quaternion.identity, manager.activeMinigame) as GameObject;
        GameObject otherCauldronObj = Instantiate(otherCauldron, new Vector3(0.0f, -1.8f, 0.0f), Quaternion.identity, manager.activeMinigame) as GameObject;
        otherCauldronObj.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        for (int c = 0; c < 3; c++)
        {
            ingredientObjs[c] = Instantiate(manager.recipe.ingredients[c], new Vector3(-8, 2 - (c * 2), 0), Quaternion.identity, manager.activeMinigame) as Ingredient;
            ingredientObjs[c].transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }
        stirringDoneAmount = 0f;
        lastPos = new Vector2(10000, 10000);
        source = GetComponent<AudioSource>();
        source.Play();
        timing = false;
        revealTimer = 0f;
        potionScoreAmount = 0f;
        canClickDone = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!OverallManager.paused)
        {
            if (timing)
            {
                Time.timeScale = 0;
                revealTimer += Time.unscaledDeltaTime;
                if (revealTimer > 3f)
                {
                    timing = false;
                    manager.EndPotion();
                }
            }
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z += 10;
            ladleObj.transform.position = mousePos;
            if (carryingIngredient)
            {
                carriedIngredient.transform.position = mousePos;
            }
            if (Input.GetKey(KeyCode.Mouse0) && Vector2.Distance(mousePos, lastPos) > 2f && cauldronObj.GetComponent<Collider2D>().bounds.Contains(mousePos))
            {
                if (ingredientsAdded > 0)
                {
                    lastPos = mousePos;
                    if (stirringDoneAmount == 0)
                    {
                        potionScoreAmount += Time.deltaTime;
                        if (potionScoreAmount > .1f)
                        {
                            potionScoreAmount = 0f;
                            stirringDoneAmount = 1f;
                            cauldronObj.GetComponent<SpriteRenderer>().color = manager.recipe.colors[ingredientsAdded - 1];
                            //switch (manager.recipe.colors[ingredientsAdded - 1])
                            //{
                            //    case Colors.Blue:
                            //        cauldronObj.GetComponent<SpriteRenderer>().color = Color.blue;
                            //        break;
                            //    case Colors.Green:
                            //        cauldronObj.GetComponent<SpriteRenderer>().color = Color.green;
                            //        break;
                            //    case Colors.Orange:
                            //        cauldronObj.GetComponent<SpriteRenderer>().color = new Color32(255, 165, 0, 255);
                            //        break;
                            //    case Colors.Purple:
                            //        cauldronObj.GetComponent<SpriteRenderer>().color = new Color32(255, 0, 255, 255);
                            //        break;
                            //    case Colors.Red:
                            //        cauldronObj.GetComponent<SpriteRenderer>().color = Color.red;
                            //        break;
                            //    case Colors.Yellow:
                            //        cauldronObj.GetComponent<SpriteRenderer>().color = Color.yellow;
                            //        break;
                            //    default:
                            //        break;
                            //}
                        }
                    }
                    else
                    {
                        stirringDoneAmount += Time.deltaTime * 3;
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Space) && canClickDone)
            {
                AudioSource.PlayClipAtPoint(tick, ladleObj.transform.position);
                float scoreReduction = Mathf.Abs(stirringDoneAmount - 1.0f) / 3.0f;
                if (scoreReduction > 1.0f / 3.0f)
                {
                    scoreReduction = 1.0f / 3.0f;
                }
                score -= scoreReduction;
                score -= (float)(3.0f - ingredientsAdded) / 3.0f;
                if (score < 0f)
                {
                    score = 0f;
                }
                scoreText.enabled = true;
                scoreText.text = "Score: " + (int)(score * 100) + "%";
                timing = true;
                manager.mixingScore = score;
                canClickDone = false;
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (carryingIngredient)
                {
                    if (cauldronObj.GetComponent<Collider2D>().bounds.Contains(mousePos))
                    {
                        AudioSource.PlayClipAtPoint(splash, ladleObj.transform.position);
                        lastPos = new Vector2(10000, 10000);
                        if (manager.recipe.ingredients[ingredientsAdded].id != carriedIngredient.id)
                        {
                            score -= 1.0f / 3.0f;
                            stirringDoneAmount = 0f;
                        }
                        else if (ingredientsAdded > 0)
                        {
                            float scoreReduction = Mathf.Abs(stirringDoneAmount - 1.0f) / 3.0f;
                            Debug.Log(scoreReduction);
                            if (scoreReduction > 1.0f / 3.0f)
                            {
                                scoreReduction = 1.0f / 3.0f;
                            }
                            score -= scoreReduction;
                            stirringDoneAmount = 0f;
                        }
                        carryingIngredient = false;
                        ingredientsAdded++;
                        carriedIngredient.transform.position = new Vector3(10000, 10000, 10000);
                        carriedIngredient = null;
                    }
                    else
                    {
                        AudioSource.PlayClipAtPoint(tick, ladleObj.transform.position);
                        bool found = false;
                        foreach (Ingredient i in ingredientObjs)
                        {
                            if (i.GetComponent<Collider2D>().bounds.Contains(mousePos) && i.id != carriedIngredient.id)
                            {
                                found = true;
                                carriedIngredient = i;
                                break;
                            }
                        }
                        if (!found)
                        {
                            carryingIngredient = false;
                            carriedIngredient = null;
                        }
                    }
                }
                else
                {
                    foreach (Ingredient i in ingredientObjs)
                    {
                        if (i.GetComponent<Collider2D>().bounds.Contains(mousePos))
                        {
                            AudioSource.PlayClipAtPoint(tick, ladleObj.transform.position);
                            carryingIngredient = true;
                            carriedIngredient = i;
                            break;
                        }
                    }
                }
            }
            
        }
    }
}
