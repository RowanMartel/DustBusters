using System.Collections.Generic;
using UnityEngine;

public class FloorMess : Interactable
{
    [Tooltip("Put the amount of splats the player needs to clean here")]
    public int int_splats;
    [HideInInspector] public bool bl_gameActive = false;
    bool bl_clean = false;

    public List<FloorSplat> l_floorSplat;

    // toggles the floor cleaning minigame if the player is holding the right object
    public override void Interact()
    {
        if (GameManager.playerController.Go_heldObject == null ||
            !GameManager.playerController.Go_heldObject.GetComponent<Pickupable>().bl_mop)
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

    public void GhostDirty(int int_aggression)
    {
        if (!bl_clean ||
            bl_gameActive ||
            GameManager.taskManager.li_taskList.Contains(TaskManager.Task.FindKey) ||
            GameManager.taskManager.li_taskList.Contains(TaskManager.Task.EscapeHouse) ||
            int_aggression < 2)
            return;

        bl_clean = false;

        bool bl_bloody = false;
        FloorSplat floorSplat = null;

        if (int_aggression >= 3)
            bl_bloody = true;

        int int_rand = Random.Range(0, int_splats);
        floorSplat = l_floorSplat[int_rand];
        GameManager.taskManager.AddTask(TaskManager.Task.MopFloor);
        GameManager.ghost.AddTask(TaskManager.Task.MopFloor);
        GameManager.ghost.RemoveTask(TaskManager.Task.GhostDirtyFloor);

        floorSplat.ReDirty(bl_bloody);
    }
}