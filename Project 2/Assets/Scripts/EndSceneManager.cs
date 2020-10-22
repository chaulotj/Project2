using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static float score;
    public Text scoreText;
    public Text message;
    public AudioClip winClip;
    public AudioClip lossClip;
    public Sprite winBg;
    public Sprite lossBg;
    public GameObject background;
    void Start()
    {
        if(score >= .7f)
        {
            message.text = "You passed!";
            background.GetComponent<SpriteRenderer>().sprite = winBg;
            AudioSource.PlayClipAtPoint(winClip, transform.position);
        }
        else
        {
            message.text = "You failed!";
            background.GetComponent<SpriteRenderer>().sprite = lossBg;
            AudioSource.PlayClipAtPoint(lossClip, transform.position);
        }
        scoreText.text = "Score: " + (int)(score * 100) + "%";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
