using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiquidSceneManager : MonoBehaviour
{
    public GameObject anvil;
    private GameObject anvilObj;
    private Collider2D anvilCollider;
    public LiquidIngredient ingredient;
    private LiquidIngredient ingredientObj;
    private Collider2D ingredientCollider;
    private float xAccel;
    public Text scoreText;
    private bool update;
    public AudioClip splat;
    // Start is called before the first frame update
    void Start()
    {
        anvilObj = Instantiate(anvil, new Vector3(Random.Range(-8.0f, 8.0f), 5, -1), Quaternion.identity) as GameObject;
        anvilCollider = anvilObj.GetComponent<Collider2D>();
        ingredientObj = Instantiate(ingredient, new Vector3(0, -4, 0), Quaternion.identity) as LiquidIngredient;
        ingredientCollider = ingredientObj.GetComponent<Collider2D>();
        xAccel = 0f;
        update = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!OverallManager.paused)
        {
            if (update)
            {
                if (anvilObj.transform.position.y - anvilCollider.bounds.extents.y > ingredientObj.transform.position.y - ingredientCollider.bounds.extents.y)
                {
                    Vector3 newPosition = anvilObj.transform.position;
                    newPosition.y -= Time.deltaTime / 2;
                    if (Input.GetKey(KeyCode.A))
                    {
                        xAccel -= Time.deltaTime / 4;
                    }
                    if (Input.GetKey(KeyCode.D))
                    {
                        xAccel += Time.deltaTime / 4;
                    }
                    if (Input.GetKey(KeyCode.S))
                    {
                        newPosition.y -= Time.deltaTime * 2;
                    }
                    newPosition.x += xAccel;
                    if (newPosition.x + anvilCollider.bounds.extents.x > 9.0f)
                    {
                        newPosition.x = 9.0f - anvilCollider.bounds.extents.x;
                    }
                    else if (newPosition.x - anvilCollider.bounds.extents.x < -9.0f)
                    {
                        newPosition.x = -9.0f + anvilCollider.bounds.extents.x;
                    }
                    anvilObj.transform.position = newPosition;
                    xAccel *= .995f;
                }
                else
                {
                    anvilObj.transform.position = new Vector3(anvilObj.transform.position.x, ingredientObj.transform.position.y - ingredientCollider.bounds.extents.y + anvilCollider.bounds.extents.y, anvilObj.transform.position.z);
                    float dist = ingredientCollider.bounds.extents.x + anvilCollider.bounds.extents.x;
                    ingredientObj.percentageGrade = Mathf.Abs(anvilObj.transform.position.x - dist) / dist;
                    if (ingredientObj.percentageGrade > 1f)
                    {
                        if (ingredientObj.percentageGrade >= 2f)
                        {
                            ingredientObj.percentageGrade = 0f;
                        }
                        else
                        {
                            ingredientObj.percentageGrade = 2f - ingredientObj.percentageGrade;
                        }
                    }
                    scoreText.enabled = true;
                    scoreText.text = "Score: " + (int)(ingredientObj.percentageGrade * 100) + "%";
                    ingredientObj.transform.position = Vector3.zero;
                    ingredientObj.GetComponent<SpriteRenderer>().sprite = ingredient.finishedImage;
                    AudioSource.PlayClipAtPoint(splat, transform.position);
                    update = false;
                }
            }
        }
    }
}
