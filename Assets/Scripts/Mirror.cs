using System.Collections.Generic;
using UnityEngine;

public class Mirror : Interactable
{
    [Tooltip("Put the amount of splats the player needs to clean here")]
    public int int_splats;
    [HideInInspector] public bool bl_gameActive = false;
    bool bl_clean = false;

    public List<MirrorSplat> l_mirrorSplat;
    public MirrorSplat bloodText;

    // toggles the mirror cleaning minigame if the player is holding the right object
    public override void Interact()
    {
        if (GameManager.playerController.Go_heldObject == null ||
            !GameManager.playerController.Go_heldObject.GetComponent<Pickupable>().bl_duster)
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
            GameManager.taskManager.CompleteTask(TaskManager.Task.CleanMirror);
            Cursor.lockState = CursorLockMode.Locked;
            GameManager.playerController.En_state = PlayerController.State.active;
            bl_gameActive = false;
            bl_clean = true;
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
        MirrorSplat mirrorSplat = null;

        if (int_aggression >= 3)
        {
            bl_bloody = true;
            bloodText?.ReDirty();
        }

        int int_rand = Random.Range(0, int_splats);
        mirrorSplat = l_mirrorSplat[int_rand];
        GameManager.taskManager.AddTask(TaskManager.Task.CleanMirror);
        GameManager.ghost.AddTask(TaskManager.Task.CleanMirror);
        GameManager.ghost.RemoveTask(TaskManager.Task.GhostDirtyMirror);

        mirrorSplat.ReDirty(bl_bloody);
    }
}