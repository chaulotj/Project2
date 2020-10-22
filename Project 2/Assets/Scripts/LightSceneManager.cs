using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightSceneManager : MonoBehaviour
{
    private LightIngredient ingredientObj;
    private List<Vector2> points;
    public LineRenderer[] lines;
    private LineRenderer[] lineObjs;
    private bool lineFollowingMouse;
    private int numClicks;
    public Text scoreText;
    private bool timing;
    private float revealTimer;
    public AudioClip tick;
    public AudioClip slice;
    private AudioSource source;
    public OverallManager manager;
    private Vector3[] arrowPositions;
    public GameObject arrow;
    private GameObject arrowObj;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<OverallManager>();
        lineFollowingMouse = false;
        lineObjs = new LineRenderer[4];
        ingredientObj = Instantiate(manager.recipe.ingredients[manager.curMinigame], new Vector3(0,0,1), Quaternion.identity, manager.activeMinigame) as LightIngredient;
        for(int c = 0; c < 4; c++)
        {
            lineObjs[c] = Instantiate(lines[c], Vector3.zero, Quaternion.identity, manager.activeMinigame) as LineRenderer;
            lineObjs[c].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineObjs[c].receiveShadows = true;
            lineObjs[c].startWidth = .05f;
            lineObjs[c].endWidth = .05f;
            lineObjs[c].enabled = false;
        }
        numClicks = 0;
        points = new List<Vector2>();
        timing = false;
        revealTimer = 0f;
        source = GetComponent<AudioSource>();
        source.volume = 1.0f;
        source.clip = tick;
        if (!manager.lightScenePlayed)
        {
            arrowPositions = new Vector3[8];
            for(int c = 0; c < 8; c++)
            {
                Vector3 curPos = new Vector3();
                curPos.z = -1f;
                if(c < 2)
                {
                    curPos.x = ingredientObj.GetComponent<Collider2D>().bounds.extents.x * -.6f;
                }
                else if(c < 4)
                {
                    curPos.x = ingredientObj.GetComponent<Collider2D>().bounds.extents.x * -.2f;
                }
                else if (c < 6)
                {
                    curPos.x = ingredientObj.GetComponent<Collider2D>().bounds.extents.x * .2f;
                }
                else if (c < 8)
                {
                    curPos.x = ingredientObj.GetComponent<Collider2D>().bounds.extents.x * .6f;
                }
                if(c % 2 == 0)
                {
                    curPos.y = ingredientObj.GetComponent<Collider2D>().bounds.extents.y + .5f;
                }
                else
                {
                    curPos.y = -ingredientObj.GetComponent<Collider2D>().bounds.extents.y - .5f;
                }
                curPos.x += arrow.GetComponent<Collider2D>().bounds.extents.x;
                curPos.y += arrow.GetComponent<Collider2D>().bounds.extents.y;
                arrowPositions[c] = curPos;
            }
            arrowObj = Instantiate(arrow, arrowPositions[0], Quaternion.identity, manager.activeMinigame);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!OverallManager.paused)
        {
            if (numClicks < 8)
            {
                Vector3[] positions = new Vector3[2];
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    points.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (numClicks % 2 == 0)
                    {
                        lineFollowingMouse = true;
                        lineObjs[numClicks / 2].enabled = true;
                    }
                    else
                    {
                        lineFollowingMouse = false;
                    }
                    numClicks++;
                    if (!manager.lightScenePlayed)
                    {
                        if (numClicks < 8)
                        {
                            arrowObj.transform.position = arrowPositions[numClicks];
                        }
                        else
                        {
                            Destroy(arrowObj);
                            manager.lightScenePlayed = true;
                        }
                    }
                    source.Play();
                }
                if (lineFollowingMouse)
                {
                    positions[0] = points[numClicks - 1];
                    positions[1] = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
                else if (numClicks > 0)
                {
                    positions[0] = points[numClicks - 2];
                    positions[1] = points[numClicks - 1];
                }
                lineObjs[(numClicks - 1) / 2].SetPositions(positions);
                if (numClicks == 8)
                {
                    float score = 1f;
                    Collider2D collider = ingredientObj.GetComponent<Collider2D>();
                    float[] fourPoints = new float[4];
                    fourPoints[3] = collider.bounds.extents.x * .6f;
                    fourPoints[2] = collider.bounds.extents.x * .2f;
                    fourPoints[1] = fourPoints[2] * -1f;
                    fourPoints[0] = fourPoints[3] * -1f;
                    float diff = collider.bounds.extents.x * .4f;
                    bool[] visited = new bool[4];
                    for (int c = 0; c < 4; c++)
                    {
                        visited[c] = false;
                    }
                    for (int c = 0; c < 4; c++)
                    {
                        int i1 = 2 * c;
                        int i2 = i1 + 1;
                        bool validLine = false;
                        if ((points[i1].y >= collider.bounds.extents.y && points[i2].y <= -collider.bounds.extents.y) || (points[i1].y <= -collider.bounds.extents.y && points[i2].y >= collider.bounds.extents.y))
                        {
                            validLine = true;
                        }
                        if (validLine)
                        {
                            float avg = (points[i1].x + points[i2].x) / 2;
                            float minDist = float.MaxValue;
                            int minDistIndex = 5;
                            for (int d = 0; d < 4; d++)
                            {
                                float tempDist = Mathf.Abs(fourPoints[d] - avg);
                                if (!visited[d] && tempDist < minDist)
                                {
                                    minDist = tempDist;
                                    minDistIndex = d;
                                }
                            }
                            visited[minDistIndex] = true;
                            float dist1 = Mathf.Abs(points[i1].x - fourPoints[minDistIndex]);
                            float dist2 = Mathf.Abs(points[i2].x - fourPoints[minDistIndex]);
                            float scoreRemoval = ((dist1 / diff) + (dist2 / diff)) / 8;
                            if (scoreRemoval > .25f)
                            {
                                scoreRemoval = .25f;
                            }
                            score -= scoreRemoval;
                        }
                        else
                        {
                            score -= .25f;
                        }
                    }
                    ingredientObj.percentageGrade = score;
                    timing = true;
                    ingredientObj.GetComponent<SpriteRenderer>().sprite = manager.recipe.ingredients[manager.curMinigame].finishedImage;
                    scoreText.enabled = true;
                    scoreText.text = "Score: " + (int)(ingredientObj.percentageGrade * 100) + "%";
                }
            }
            if (timing)
            {
                Time.timeScale = 0;
                revealTimer += .01f;
                if (revealTimer > 3f)
                {
                    timing = false;
                    Time.timeScale = 1;
                    source.clip = slice;
                    source.Play();
                    timing = false;
                    manager.recipe.ingredients[manager.curMinigame].percentageGrade = ingredientObj.percentageGrade;
                    manager.recipe.ingredients[manager.curMinigame].GetComponent<SpriteRenderer>().sprite = manager.recipe.ingredients[manager.curMinigame].finishedImage;
                    for (int c = 0; c < 4; c++)
                    {
                        lineObjs[c].enabled = false;
                    }
                    manager.curMinigame++;
                    if(manager.curMinigame == 3)
                    {
                        manager.playMinigame();
                    }
                    else
                    {
                        manager.playMinigame(manager.recipe.ingredients[manager.curMinigame]);
                    }
                }
            }
        }
    }
}
