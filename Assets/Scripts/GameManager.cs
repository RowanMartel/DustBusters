using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public static TaskManager taskManager;
    public static PlayerController playerController;
    public static GhostBehavior ghost;
    public static MenuManager menuManager;
    public static HealthSystem healthSystem;
    private void Awake()
    {
        gameManager = this;
        menuManager = FindObjectOfType<MenuManager>();
        taskManager = GetComponent<TaskManager>();
        playerController = FindObjectOfType<PlayerController>();
        healthSystem = playerController.GetComponent<HealthSystem>();
        ghost = FindAnyObjectByType<GhostBehavior>();
    }

    public static void ResetGame()
    {
        healthSystem.Int_playerHealth = 3;

        Color tempcolor = menuManager.Img_damageOverlay.color;
        tempcolor.a = 0;
        menuManager.Img_damageOverlay.color = tempcolor;
    }
}