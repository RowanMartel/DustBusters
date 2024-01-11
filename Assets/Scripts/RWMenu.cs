using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RWMenu : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject optionsScreen;
    public GameObject gameScreen;
    public GameObject pauseScreen;

    public static RWMenu instance;

    private void Start()
    {
        if(instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // SwitchScreen(titleScreen);
    }

    public void SwitchScreen(GameObject screen)
    {
        titleScreen.SetActive(false);
        optionsScreen.SetActive(false);
        gameScreen.SetActive(false);
        pauseScreen.SetActive(false);

        if (screen != pauseScreen) Time.timeScale = 1;

        screen.SetActive(true);
    }

    public void EnterScene(int index)
    {
        SceneManager.LoadScene(index);
        SwitchScreen(gameScreen);
    }

    public void ToTitle()
    {
        SceneManager.LoadScene(0);
        SwitchScreen(titleScreen);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameScreen.activeSelf)
            {
                SwitchScreen(pauseScreen);
                Time.timeScale = 0;
            }
            else if (pauseScreen.activeSelf)
            {
                SwitchScreen(gameScreen);
                //Time.timeScale = 1;
            }
        }
    }
}
