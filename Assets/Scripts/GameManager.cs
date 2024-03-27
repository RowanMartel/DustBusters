using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public static TaskManager taskManager;
    public static PlayerController playerController;
    public static GhostBehavior ghost;
    public static MenuManager menuManager;
    public static HealthSystem healthSystem;
    public static SoundManager soundManager;

    public static GameManager instance;

    // keeps track of if the player is in a cleaning minigame
    private static bool bl_inCleaningGame;
    public static bool Bl_inCleaningGame { get { return bl_inCleaningGame; } set { bl_inCleaningGame = value; } }

    private void Awake()
    {
        //Singleton
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

        taskManager = GetComponent<TaskManager>();
        soundManager = GetComponent<SoundManager>();
        if(menuManager == null) menuManager = FindObjectOfType<MenuManager>();
        menuManager.InitializeMenuManager();

        SceneManager.activeSceneChanged += OnGameStart;

        menuManager.FadeIn();
    }

    // assigns neccessary variables on scene change
    public void OnGameStart(Scene oldScene, Scene newScene)
    {
        if (instance != this) return;

        soundManager = GetComponent<SoundManager>();
        taskManager.ResetValues();

        Bl_inCleaningGame = false;
    }

    public static void ResetGame()
    {
        menuManager.ResetMenus();
    }
}