using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : Interactable
{
    [Tooltip("Put the amount of splats the player needs to clean here")]
    public int splats;
    [HideInInspector] public bool gameActive = false;
    bool clean = false;

    public override void Interact()
    {
        // if (!player.heldItem.duster) return;

        if (clean) return;

        gameActive = !gameActive;

        // if (gameActive)
            // unpull focus();
        // else
            // pull focus();
    }

    public void CleanSplat()
    {
        splats--;
        if (splats <= 0)
        {
            GameManager.taskManager.CompleteTask(TaskManager.Task.CleanMirror);
            // unpull focus();
            clean = true;
        }
    }
}