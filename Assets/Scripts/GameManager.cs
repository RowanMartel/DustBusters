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
    private void Awake()
    {
        taskManager = GetComponent<TaskManager>();
        soundManager = GetComponent<SoundManager>();
        menuManager = FindObjectOfType<MenuManager>();

        //Singleton
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            menuManager.FadeIn();
        }
        else Destroy(gameObject);

        SceneManager.activeSceneChanged += OnGameStart;
    }

    // assigns neccessary variables on scene change
    public void OnGameStart(Scene oldScene, Scene newScene)
    {
        if (instance != this) return;

        //taskManager = GetComponent<TaskManager>();
        //soundManager = GetComponent<SoundManager>();
        //menuManager = FindObjectOfType<MenuManager>();
        //playerController = FindObjectOfType<PlayerController>();
        //healthSystem = playerController?.GetComponent<HealthSystem>();
        //ghost = FindAnyObjectByType<GhostBehavior>();
        taskManager.ResetValues();
    }

    public static void ResetGame()
    {
        menuManager.ResetMenus();
    }
}