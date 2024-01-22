using UnityEngine;

public class FloorMess : Interactable
{
    [Tooltip("Put the amount of splats the player needs to clean here")]
    public int int_splats;
    [HideInInspector] public bool bl_gameActive = false;
    bool bl_clean = false;

    // toggles the floor cleaning minigame if the player is holding the right object
    public override void Interact()
    {
        if (GameManager.playerController.Go_heldObject == null ||
            !GameManager.playerController.Go_heldObject.GetComponent<Pickupable>().mop)
            return;

        if (bl_clean) return;

        bl_gameActive = !bl_gameActive;

        if (bl_gameActive)
        {
            Cursor.lockState = CursorLockMode.Confined;
            GameManager.playerController.En_state = PlayerController.State.inactive;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            GameManager.playerController.En_state = PlayerController.State.active;
        }
    }

    // ticks down splats int, then checks if the minigame is complete
    public void CleanSplat()
    {
        int_splats--;
        if (int_splats <= 0)
        {
            GameManager.taskManager.CompleteTask(TaskManager.Task.MopFloor);
            Cursor.lockState = CursorLockMode.Locked;
            GameManager.playerController.En_state = PlayerController.State.active;
            bl_gameActive = false;
            bl_clean = true;
        }
    }
}