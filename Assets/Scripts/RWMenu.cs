using UnityEngine;
using UnityEngine.SceneManagement;

public class RWMenu : MonoBehaviour
{
    //Game Screens
    public GameObject titleScreen;
    public GameObject optionsScreen;
    public GameObject gameScreen;
    public GameObject pauseScreen;
    public GameObject deathScreen;
    public GameObject startScreen;
    public GameObject endScreen;

    //Singleton
    public static RWMenu instance;

    private void Awake()
    {
        //Singleton
        if(instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        //Set up
        SwitchScreen(titleScreen);
    }

    //Switch the currently displayed screen to be the designated screen
    public void SwitchScreen(GameObject screen)
    {
        titleScreen.SetActive(false);
        optionsScreen.SetActive(false);
        gameScreen.SetActive(false);
        pauseScreen.SetActive(false);
        deathScreen.SetActive(false);
        startScreen.SetActive(false);
        endScreen.SetActive(false);

        if (screen != pauseScreen && screen != startScreen)
            Time.timeScale = 1;

        screen.SetActive(true);
    }

    //Switch to Death Screen
    public void ShowDeathScreen()
    {
        deathScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0;
    }

    //Enter the Start Scene
    public void EnterScene(int index)
    {
        SceneManager.LoadScene(index);

        SwitchScreen(startScreen);

        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;

    }

    //Start Game
    public void StartGame()
    {
        SwitchScreen(gameScreen);

        Cursor.lockState = CursorLockMode.Locked;

        GameManager.playerController.TogglePlayerControl();
    }

    //Go To Title Screen
    public void ToTitle()
    {
        SceneManager.LoadScene(0);
        SwitchScreen(titleScreen);
    }

    //Go to End Scene
    public void ToEnd()
    {
        SceneManager.LoadScene(2);
        SwitchScreen(endScreen);
        Cursor.lockState = CursorLockMode.Confined;
    }

    //Close the game
    public void Quit()
    {
        Application.Quit();
    }

    //Go to Gameplay from Pause
    public void Unpause()
    {
        SwitchScreen(gameScreen);
        GameManager.playerController.TogglePlayerControl();
    }

    private void Update()
    {
        //Toggle Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameScreen.activeSelf)
            {
                SwitchScreen(pauseScreen);
                GameManager.playerController.TogglePlayerControl();
                Time.timeScale = 0;
            }
            else if (pauseScreen.activeSelf)
            {
                SwitchScreen(gameScreen);
                GameManager.playerController.TogglePlayerControl();
                //Time.timeScale = 1;
            }
        }
    }
}
