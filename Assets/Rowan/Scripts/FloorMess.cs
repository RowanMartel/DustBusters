using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMess : Interactable
{
    [Tooltip("Put the amount of splats the player needs to clean here")]
    public int splats;
    [HideInInspector] public bool gameActive = false;
    bool clean = false;

    public override void Interact()
    {
        if (GameManager.playerController.heldObject == null || !GameManager.playerController.heldObject.GetComponent<Pickupable>().mop) return;

        if (clean) return;

        gameActive = !gameActive;

        if (gameActive)
        {
            Cursor.lockState = CursorLockMode.Confined;
            GameManager.playerController.state = PlayerController.State.inactive;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            GameManager.playerController.state = PlayerController.State.active;
        }
    }

    public void CleanSplat()
    {
        splats--;
        if (splats <= 0)
        {
            GameManager.taskManager.CompleteTask(TaskManager.Task.MopFloor);
            Cursor.lockState = CursorLockMode.Locked;
            GameManager.playerController.state = PlayerController.State.active;
            gameActive = false;
            clean = true;
        }
    }
}