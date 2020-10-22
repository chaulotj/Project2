using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResumeButton()
    {
        OverallManager.Pause();
    }

    public void MenuButton()
    {
        OverallManager.paused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("MenuScene");
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void StartButton()
    {
        SceneManager.LoadScene("MainScene");
    }
}
