using System;
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

    public Texture2D broomPointer;

    [HideInInspector] public bool bl_paused = false;
    bool bl_frameClicked = false;

    private void Awake()
    {
        GameManager.menuManager.GamePaused += OnPause;
        GameManager.menuManager.GameUnpaused += OnUnpause;
    }

    // toggles the floor cleaning minigame if the player is holding the right object
    public override void Interact()
    {
        if (GameManager.playerController.Go_heldObject == null ||
            !GameManager.playerController.Go_heldObject.GetComponent<Pickupable>().bl_mop ||
            bl_frameClicked)
            return;

        if (bl_clean) return;

        bl_frameClicked = true;

        bl_gameActive = !bl_gameActive;

        // cinemachine brain goes to the minigame cam when toggled on, and the player cam when toggled off
        go_virtualCam.SetActive(!go_virtualCam.activeSelf);

        GameManager.Bl_inCleaningGame = !GameManager.Bl_inCleaningGame;
        GameManager.playerController.TogglePlayerControl();
        GameManager.menuManager.img_crosshair.enabled = !GameManager.menuManager.img_crosshair.enabled;

        if (bl_gameActive)
        {
            GameManager.playerController.DisablePhysics();
            Cursor.SetCursor(broomPointer, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            GameManager.playerController.EnablePhysics();
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)) && bl_gameActive && !GameManager.menuManager.Bl_paused)
        {
            Interact();
        }
        if (bl_frameClicked) bl_frameClicked = false;
    }

    // ticks down splats int, then checks if the minigame is complete
    public void CleanSplat()
    {
        int_splats--;
        if (int_splats <= 0)
        {
            GameManager.taskManager.CompleteTask(TaskManager.Task.MopFloor);
            GameManager.Bl_inCleaningGame = false;
            GameManager.playerController.TogglePlayerControl();
            GameManager.playerController.EnablePhysics();
            bl_gameActive = false;
            bl_clean = true;
            go_virtualCam.SetActive(false); // return to player virtual cam
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            GameManager.menuManager.img_crosshair.enabled = true;
        }
    }

    // makes a splat dirty again and reassigns the task
    public void GhostDirty(int int_aggression)
    {
        if (!bl_clean ||
            bl_gameActive ||
            bl_paused ||
            GameManager.taskManager.li_taskList.Contains(TaskManager.Task.FindKey) ||
            GameManager.taskManager.li_taskList.Contains(TaskManager.Task.EscapeHouse) ||
            int_aggression < 2)
            return;

        bl_clean = false;

        bool bl_bloody = false;
        FloorSplat floorSplat = null;

        if (int_aggression >= 3)
            bl_bloody = true;

        int int_rand = UnityEngine.Random.Range(0, int_splats);
        floorSplat = l_floorSplat[int_rand];
        GameManager.taskManager.AddTask(TaskManager.Task.MopFloor);
        GameManager.ghost.AddTask(TaskManager.Task.MopFloor);
        GameManager.ghost.RemoveTask(TaskManager.Task.GhostDirtyFloor);

        floorSplat.ReDirty(bl_bloody);
    }

    // handles cursor state when the player pauses while in the minigame. receives an event from menumanager
    void OnPause(object source, EventArgs e)
    {
        if (!bl_gameActive) return;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        bl_paused = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    // handles cursor state when the player unpauses while in the minigame. receives an event from menumanager
    void OnUnpause(object source, EventArgs e)
    {
        if (!bl_gameActive) return;
        Cursor.SetCursor(broomPointer, Vector2.zero, CursorMode.Auto);
        bl_paused = false;
    }
}