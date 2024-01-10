using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Pickupable
{
    public override void Interact()
    {
        if (GameManager.taskManager.taskList.Contains(TaskManager.Task.FindKey))
            GameManager.taskManager.CompleteTask(TaskManager.Task.FindKey);
    }
}