using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public static TaskManager taskManager;

    private void Awake()
    {
        gameManager = this;
        taskManager = GetComponentInChildren<TaskManager>();
    }
}