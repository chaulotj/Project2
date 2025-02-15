﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
    //for drawing line objects
    public GameObject lineGO;
    //public GameObject currLine;
    public LineRenderer lineRenderer;
    public EdgeCollider2D edgeCollider;

    //keep track of user input & intended path
    public List<Vector2> mousePositions;
    public List<Vector2> squarePath;
    public List<Vector2> pathComparison;

    //temporary vect for loops
    Vector2 temp;

    public float scoreFin = 0f;
    public bool showScore = false;

    //SFX
    public AudioSource source;
    public AudioClip[] sounds;
    private bool timing;

    public OverallManager manager;
    public SolidIngredient ingredient;
    public SpriteRenderer sprite;
    private List<GameObject> lines;
    int lineCount;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<OverallManager>();
        ingredient = Instantiate(manager.recipe.ingredients[manager.curMinigame], new Vector3(0, 0, 1), Quaternion.identity, manager.activeMinigame) as SolidIngredient;
        sprite = ingredient.GetComponent<SpriteRenderer>();

        lines = new List<GameObject>();
        lineCount =0;

        source = gameObject.GetComponent<AudioSource>();

        //top left to top right
        float counter = -3.0f;
        while(counter <= 3)
        {
            temp = new Vector2(counter, 3);
            squarePath.Add(temp);
            counter += .3f;
            //Debug.Log("1" + temp);
        }
        //top right to bottom right
        counter = 3.0f;
        while (counter >= -3)
        {
            temp = new Vector2(3, counter);
            squarePath.Add(temp);
            counter -= .3f;
            //Debug.Log("2" + temp);
        }
        //bottom right to bottom left
        counter = 3.0f;
        while (counter >= -3)
        {
            temp = new Vector2(counter, -3);
            squarePath.Add(temp);
            counter -= .3f;
            //Debug.Log("3" + temp);
        }
        //bottom left to top left
        counter = -3.0f;
        while (counter <= 3)
        {
            temp = new Vector2(-3, counter);
            squarePath.Add(temp);
            counter += .3f;
            //Debug.Log("4" + temp);
        }
        timing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!OverallManager.paused)
        {
            //only call function once left mouse button is clicked
            if (Input.GetMouseButtonDown(0))
            {
                //SFX
                source.clip = sounds[Random.Range(0, sounds.Length)];
                source.Play();
                CreateLine();
            }
            //and while it's being held down
            if (Input.GetMouseButton(0))
            {
                Vector2 tempMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (Vector2.Distance(tempMousePos, mousePositions[mousePositions.Count - 1]) > .1f)
                {
                    UpdateLine(tempMousePos);
                }
            }
            //score path
            if (Input.GetKeyDown(KeyCode.S))
            {
                timing = true;
                scoreFin = ScorePath();
                if(scoreFin > 1f)
                {
                    scoreFin = 1f;
                }
                //scoreFin /= 100;

                ingredient.percentageGrade = scoreFin;
                manager.recipe.ingredients[manager.curMinigame].percentageGrade = ingredient.percentageGrade;
                showScore = true;
            }
            if (timing)
            {
                Time.timeScale = 0f;
            }
            if (showScore && Input.GetKeyDown(KeyCode.Space))
            {
                Time.timeScale = 1f;
                timing = false;
                foreach (GameObject line in lines)
                {
                    Destroy(line);
                }
                playNext();
            }
        }
    }

    void CreateLine()
    {

        lines.Add(Instantiate(lineGO, Vector3.zero, Quaternion.identity));

        lineRenderer = lines[lineCount].GetComponent<LineRenderer>();
        edgeCollider = lines[lineCount].GetComponent<EdgeCollider2D>();
        lineCount++;

        //user input
        mousePositions.Clear();
        mousePositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        mousePositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        lineRenderer.SetPosition(0, mousePositions[0]);
        lineRenderer.SetPosition(1, mousePositions[1]);
        edgeCollider.points = mousePositions.ToArray();

    }

    void UpdateLine(Vector2 newMousePos)
    {
        mousePositions.Add(newMousePos);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount-1, newMousePos);
        edgeCollider.points = mousePositions.ToArray();
    }

    float ScorePath()
    {
       float score = 0f;
        //score line drawn
        foreach (Vector2 intendedPt in squarePath)
        {
            foreach (Vector2 mousePt in mousePositions)
            {
                //within range - x
                if(intendedPt.x - .2 < mousePt.x && mousePt.x < intendedPt.x + .2)
                {
                    //within range - y
                    if (intendedPt.y - .2 < mousePt.y && mousePt.y < intendedPt.y + .2)
                    {
                        score += 1f;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
                
            }
        }
        score /= 84f;
        //if (score > 100)
        //{
        //    float cut = score / 100f;
        //    Debug.Log("Cut: " + cut);
        //    score /= cut;
        //    Debug.Log("Score/cut: " + score);
        //}
        //Debug.Log("Fin score: " + score);
        return score;
    }

    void playNext()
    {
        manager.recipe.ingredients[manager.curMinigame].GetComponent<SpriteRenderer>().sprite = sprite.sprite;
        timing = false;
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
}
