using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustIngredient : Ingredient
{

	public float score;

	public const int MAX_CLICKS = 5;
	public int clickCounter;

	public Color dustColor;
	public SpriteRenderer s_renderer;
	public GameObject marker;
	public float height;
	public float width;
	private List<Vector3> points;
	public Vector3 mouseClickPos;
	private Vector3 tempPoint;
	private float minDistance;
	private float tempDistance;

	public float distance;
    // Start is called before the first frame update
    void Start()
    {
		height = s_renderer.bounds.size.y;
		width = s_renderer.bounds.size.x;
		minDistance = float.MaxValue;
		points = new List<Vector3>();

		points.Add(new Vector3(gameObject.transform.position.x - width / 4, gameObject.transform.position.y + height / 4,-1));
		points.Add(new Vector3(gameObject.transform.position.x + width / 4, gameObject.transform.position.y + height / 4,-1));
		points.Add(new Vector3(gameObject.transform.position.x - width / 4, gameObject.transform.position.y - height / 4,-1));
		points.Add(new Vector3(gameObject.transform.position.x + width / 4, gameObject.transform.position.y - height / 4,-1));
		points.Add(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -1));

		foreach (Vector3 point in points) {
			Instantiate(marker, point, Quaternion.identity);
		}
	}

	// Update is called once per frame
	void Update()
    {
		if (clickCounter<MAX_CLICKS && Input.GetMouseButtonDown(0)) {
			mouseClickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			clickCounter++;
			minDistance = float.MaxValue;

			foreach (Vector3 point in points) {
				tempDistance = Vector2.Distance(mouseClickPos, point);
				if (tempDistance < minDistance) {
					minDistance = tempDistance;
					tempPoint = point;
				}
			}
			tempPoint.z = 10;

			distance = minDistance;

			//add the score
			if (minDistance < 0.5f) score += 100;
			else if (minDistance < 1.0f) score += 90;
			else if (minDistance < 1.5f) score += 80;
			else if (minDistance < 2.0f) score += 70;
			else if (minDistance < 2.5f) score += 60;
			else if (minDistance < 3.0f) score += 50;
			else score += 0;

			points.Remove(tempPoint);

		}
    }
}
