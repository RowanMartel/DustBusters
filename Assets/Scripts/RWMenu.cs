using UnityEngine;
using UnityEngine.SceneManagement;

public class RWMenu : MonoBehaviour
{
    //Game Screens
    public GameObject go_titleScreen;
    public GameObject go_optionsScreen;
    public GameObject go_gameScreen;
    public GameObject go_pauseScreen;
    public GameObject go_deathScreen;
    public GameObject go_startScreen;
    public GameObject go_endScreen;

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
        SwitchScreen(go_titleScreen);
    }

    //Switch the currently displayed screen to be the designated screen
    public void SwitchScreen(GameObject screen)
    {
        go_titleScreen.SetActive(false);
        go_optionsScreen.SetActive(false);
        go_gameScreen.SetActive(false);
        go_pauseScreen.SetActive(false);
        go_deathScreen.SetActive(false);
        go_startScreen.SetActive(false);
        go_endScreen.SetActive(false);

        if (screen != go_pauseScreen && screen != go_startScreen)
            Time.timeScale = 1;

        screen.SetActive(true);
    }

    //Switch to Death Screen
    public void ShowDeathScreen()
    {
        go_deathScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0;
    }

    //Enter the Start Scene
    public void EnterScene(int index)
    {
        SceneManager.LoadScene(index);

        SwitchScreen(go_startScreen);

        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;

    }

    //Start Game
    public void StartGame()
    {
        SwitchScreen(go_gameScreen);

        Cursor.lockState = CursorLockMode.Locked;

        GameManager.playerController.TogglePlayerControl();
    }

    //Go To Title Screen
    public void ToTitle()
    {
        SceneManager.LoadScene(0);
        SwitchScreen(go_titleScreen);
    }

    //Go to End Scene
    public void ToEnd()
    {
        SceneManager.LoadScene(2);
        SwitchScreen(go_endScreen);
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
        SwitchScreen(go_gameScreen);
        GameManager.playerController.TogglePlayerControl();
    }

    private void Update()
    {
        //Toggle Pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (go_gameScreen.activeSelf)
            {
                SwitchScreen(go_pauseScreen);
                GameManager.playerController.TogglePlayerControl();
                Time.timeScale = 0;
            }
            else if (go_pauseScreen.activeSelf)
            {
                SwitchScreen(go_gameScreen);
                GameManager.playerController.TogglePlayerControl();
                //Time.timeScale = 1;
            }
        }
    }
}
