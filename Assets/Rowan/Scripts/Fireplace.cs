using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireplace : Interactable
{
    [Tooltip("Put the fireplace particle effect object here")]
    public GameObject fireFX;

    public override void Interact()
    {
        // if (!player.heldItem.lighter) return;
        Light();
    }

    public void Light()
    {
        fireFX.SetActive(true);

        GameManager.taskManager.CompleteTask(TaskManager.Task.LightFireplace);
    }
    public void UnLight()
    {
        fireFX.SetActive(false);

        if (GameManager.taskManager.taskList.Contains(TaskManager.Task.FindKey) ||
            GameManager.taskManager.taskList.Contains(TaskManager.Task.EscapeHouse))
            return;

        GameManager.taskManager.taskList.Add(TaskManager.Task.LightFireplace);
    }
}