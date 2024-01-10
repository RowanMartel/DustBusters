using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public static TaskManager taskManager;
    public static PlayerController playerController;

    private void Awake()
    {
        gameManager = this;
        taskManager = GetComponent<TaskManager>();
        playerController = FindObjectOfType<PlayerController>();
    }
}