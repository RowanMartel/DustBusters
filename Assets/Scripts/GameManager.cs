using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public static TaskManager taskManager;
    public static PlayerController playerController;
    public static GhostBehavior ghost;

    private void Awake()
    {
        gameManager = this;
        taskManager = GetComponent<TaskManager>();
        playerController = FindObjectOfType<PlayerController>();
        ghost = FindAnyObjectByType<GhostBehavior>();
    }

    public void ResetGame()
    {
        
    }
}