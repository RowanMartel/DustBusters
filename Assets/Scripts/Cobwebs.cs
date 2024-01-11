using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cobwebs : MonoBehaviour
{
    [Tooltip("Put the amount of cobwebs the player needs to clean here")]
    public int cobwebs;
    [HideInInspector] public bool gameActive = false;

    public void CleanWeb()
    {
        cobwebs--;
        if (cobwebs <= 0)
            GameManager.taskManager.CompleteTask(TaskManager.Task.CleanCobwebs);
    }
}