using UnityEngine;
using UnityEngine.SceneManagement;

public class RWMenu : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject optionsScreen;
    public GameObject gameScreen;
    public GameObject pauseScreen;
    public GameObject deathScreen;
    public GameObject startScreen;
    public GameObject endScreen;

    public static RWMenu instance;

    private void Awake()
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

        SwitchScreen(titleScreen);
    }

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

    public void ShowDeathScreen()
    {
        deathScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0;
    }

    public void EnterScene(int index)
    {
        SceneManager.LoadScene(index);

        SwitchScreen(startScreen);

        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;

    }

    public void StartGame()
    {
        SwitchScreen(gameScreen);

        Cursor.lockState = CursorLockMode.Locked;

        GameManager.playerController.TogglePlayerControl();
    }

    public void ToTitle()
    {
        SceneManager.LoadScene(0);
        SwitchScreen(titleScreen);
    }

    public void ToEnd()
    {
        SceneManager.LoadScene(2);
        SwitchScreen(endScreen);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Unpause()
    {
        SwitchScreen(gameScreen);
        GameManager.playerController.TogglePlayerControl();
    }
    private void Update()
    {
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
