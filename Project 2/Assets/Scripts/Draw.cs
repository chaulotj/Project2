using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
    //for drawing line objects
    public GameObject lineGO;
    public GameObject currLine;
    public LineRenderer lineRenderer;
    public EdgeCollider2D edgeCollider;

    //keep track of user input & intended path
    public List<Vector2> mousePositions;
    public List<Vector2> squarePath;
    public List<Vector2> pathComparison;

    //temporary vect for loops
    Vector2 temp;

    public int scoreFin = 0;
    public bool showScore = false;

    // Start is called before the first frame update
    void Start()
    {
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

    }

    // Update is called once per frame
    void Update()
    {
        //only call function once left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            CreateLine();
        }
        //and while it's being held down
        if (Input.GetMouseButton(0))
        {
            Vector2 tempMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(tempMousePos, mousePositions[mousePositions.Count - 1]) > .1f){
                UpdateLine(tempMousePos);
            }
        }
        //score path
        if (Input.GetKeyDown(KeyCode.S))
        {
            scoreFin = ScorePath();
            Debug.Log("ScoreFin: " + scoreFin);
            showScore = true;
        }

    }

    void CreateLine()
    {
        currLine = Instantiate(lineGO, Vector3.zero, Quaternion.identity);
        lineRenderer = currLine.GetComponent<LineRenderer>();
        edgeCollider = currLine.GetComponent<EdgeCollider2D>();

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

    int ScorePath()
    {
        int score = 0;
        //score line drawn
        foreach (Vector2 mousePt in mousePositions)
        {
            foreach (Vector2 intendedPt in squarePath)
            {
                //within range - x
                if(intendedPt.x - .1 < mousePt.x && mousePt.x < intendedPt.x + .1)
                {
                    //within range - y
                    if (intendedPt.y - .1 < mousePt.y && mousePt.y < intendedPt.y + .1)
                    {
                        score += 5;
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

        return score;
    }
}
