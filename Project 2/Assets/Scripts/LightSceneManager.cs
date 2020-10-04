using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSceneManager : MonoBehaviour
{
    public LightIngredient ingredient;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(ingredient, Vector3.zero, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
