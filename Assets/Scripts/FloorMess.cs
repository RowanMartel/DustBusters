using System.Collections.Generic;
using UnityEngine;

public class FloorMess : Interactable
{
    [Tooltip("Put the amount of splats the player needs to clean here")]
    public int int_splats;
    [HideInInspector] public bool bl_gameActive = false;
    public bool bl_clean = false;
    public GameObject go_virtualCam;

    public List<FloorSplat> l_floorSplat;

    // toggles the floor cleaning minigame if the player is holding the right object
    public override void Interact()
    {
        if (GameManager.playerController.Go_heldObject == null ||
            !GameManager.playerController.Go_heldObject.GetComponent<Pickupable>().bl_mop)
            return;

        if (bl_clean) return;

        bl_gameActive = !bl_gameActive;
        go_virtualCam.SetActive(!go_virtualCam.activeSelf);
        GameManager.menuManager.Bl_allowPause = !GameManager.menuManager.Bl_allowPause;

        GameManager.playerController.TogglePlayerControl();
    }

    // ticks down splats int, then checks if the minigame is complete
    public void CleanSplat()
    {
        int_splats--;
        if (int_splats <= 0)
        {
            GameManager.taskManager.CompleteTask(TaskManager.Task.MopFloor);
            GameManager.playerController.TogglePlayerControl();
            bl_gameActive = false;
            bl_clean = true;
            go_virtualCam.SetActive(false);
            GameManager.menuManager.Bl_allowPause = true;
        }
    }

    // makes a splat dirty again and reassigns the task
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