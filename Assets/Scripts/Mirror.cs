using System;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : Interactable
{
    [Tooltip("Put the amount of splats the player needs to clean here")]
    public int int_splats;
    [HideInInspector] public bool bl_gameActive = false;
    public bool bl_clean = false;
    public GameObject go_virtualCam;

    public List<MirrorSplat> l_mirrorSplat;
    public MirrorSplat bloodText;

    public List<float> l_flt_chanceForSpookByAggro;
    public List<GameObject> l_go_spookyThingByAggro;
    public Transform tr_spawnSpooky;

    public Texture2D dusterPointer;

    [HideInInspector] public bool bl_paused = false;

    private void Awake()
    {
        GameManager.menuManager.GamePaused += OnPause;
        GameManager.menuManager.GameUnpaused += OnUnpause;
    }

    // toggles the mirror cleaning minigame if the player is holding the right object
    public override void Interact()
    {
        if (GameManager.playerController.Go_heldObject == null ||
            !GameManager.playerController.Go_heldObject.GetComponent<Pickupable>().bl_duster)
            return;

        if (bl_clean) return;

        bl_gameActive = !bl_gameActive;

        // cinemachine brain goes to the minigame cam when toggled on, and the player cam when toggled off
        go_virtualCam.SetActive(!go_virtualCam.activeSelf);

        GameManager.Bl_inCleaningGame = !GameManager.Bl_inCleaningGame;
        GameManager.playerController.TogglePlayerControl();
        GameManager.menuManager.img_crosshair.enabled = !GameManager.menuManager.img_crosshair.enabled;

        if (bl_gameActive)
            Cursor.SetCursor(dusterPointer, Vector2.zero, CursorMode.Auto);
        else
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    // ticks down splats int, then checks if the minigame is complete
    public void CleanSplat()
    {
        int_splats--;
        if (int_splats <= 0)
        {
            GameManager.taskManager.CompleteTask(TaskManager.Task.CleanMirror);
            GameManager.Bl_inCleaningGame = false;
            GameManager.playerController.TogglePlayerControl();
            bl_gameActive = false;
            bl_clean = true;
            go_virtualCam.SetActive(false); // return to player virtual cam
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            GameManager.menuManager.img_crosshair.enabled = true;

            //Spooky Encounter
            int int_aggro = GameManager.ghost.int_curAggressionLevel - 1;
            float flt_rand = UnityEngine.Random.Range(0, 100);
            if(flt_rand <= l_flt_chanceForSpookByAggro[int_aggro])
            {
                GameObject go_spooky = Instantiate(l_go_spookyThingByAggro[int_aggro]);
                go_spooky.transform.position = tr_spawnSpooky.position;
            }

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
        MirrorSplat mirrorSplat = null;

        if (int_aggression >= 3)
        {
            bl_bloody = true;
            bloodText?.ReDirty();
        }

        int int_rand = UnityEngine.Random.Range(0, int_splats);
        mirrorSplat = l_mirrorSplat[int_rand];
        GameManager.taskManager.AddTask(TaskManager.Task.CleanMirror);
        GameManager.ghost.AddTask(TaskManager.Task.CleanMirror);
        GameManager.ghost.RemoveTask(TaskManager.Task.GhostDirtyMirror);

        mirrorSplat.ReDirty(bl_bloody);
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
        Cursor.SetCursor(dusterPointer, Vector2.zero, CursorMode.Auto);
        bl_paused = false;
    }
}