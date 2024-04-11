using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public List<Transform> l_tr_spawnSpookyByAggro;

    public Texture2D dusterPointer;

    public static bool bl_jumpscared;

    [HideInInspector] public bool bl_paused = false;
    bool bl_frameClicked = false;

    public bool bl_rotated;

    // jump scare audio variables
    [SerializeField] AudioSource as_source;
    [SerializeField] AudioClip ac_cardboardJumpscare;
    [SerializeField] AudioClip ac_scaryJumpscare;

    private void Awake()
    {
        bl_jumpscared = false;
        GameManager.menuManager.GamePaused += OnPause;
        GameManager.menuManager.GameUnpaused += OnUnpause;
    }

    // toggles the mirror cleaning minigame if the player is holding the right object
    public override void Interact()
    {
        if (GameManager.playerController.Go_heldObject == null ||
            !GameManager.playerController.Go_heldObject.GetComponent<Pickupable>().bl_soapBar ||
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
            Cursor.SetCursor(dusterPointer, Vector2.zero, CursorMode.Auto);
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
            GameManager.taskManager.CompleteTask(TaskManager.Task.CleanMirror);
            GameManager.Bl_inCleaningGame = false;
            GameManager.playerController.TogglePlayerControl();
            GameManager.playerController.EnablePhysics();
            bl_gameActive = false;
            bl_clean = true;
            go_virtualCam.SetActive(false); // return to player virtual cam
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            GameManager.menuManager.img_crosshair.enabled = true;

            //Spooky Encounter
            int int_aggro = GameManager.ghost.int_curAggressionLevel - 1;
            float flt_rand = UnityEngine.Random.Range(0, 100);
            if(flt_rand <= l_flt_chanceForSpookByAggro[int_aggro] && bl_jumpscared == false)
            {
                bl_jumpscared = true;
                GameObject go_spooky = Instantiate(l_go_spookyThingByAggro[int_aggro]);
                go_spooky.transform.position = l_tr_spawnSpookyByAggro[int_aggro].position;
                if (bl_rotated)
                {
                    //go_spooky.transform.rotation.SetEulerAngles(new Vector3(go_spooky.transform.rotation.x, go_spooky.transform.rotation.y - 90, go_spooky.transform.rotation.z));
                    go_spooky.transform.Rotate(new Vector3(go_spooky.transform.rotation.x, go_spooky.transform.rotation.y - 90, go_spooky.transform.rotation.z));
                }
                // play the corresponding noise
                if (int_aggro < 2)
                    as_source.PlayOneShot(ac_cardboardJumpscare, Settings.flt_volume);
                else
                    as_source.PlayOneShot(ac_scaryJumpscare, Settings.flt_volume);
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