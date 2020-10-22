using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public int id; //Each ingredient needs to have a different id!
    public float percentageGrade = 0f;
    public Sprite finishedImage;
    public Sprite startImage;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = startImage;
        percentageGrade = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
