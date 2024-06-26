using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    int int_tutorialSegment;
    bool bl_tutorialActive;

    // variables for segment completion detection
    Vector3 v3_prevMousePos;
    [SerializeField] LightSwitch diningSwitch;
    [SerializeField] PlayerCheckZone doorZone;
    [SerializeField] PlayerCheckZone dishZone;
    int int_mousePosChanges;
    bool bl_hasASwitchChanged;

    // game objects for the tutorial UI
    [SerializeField] GameObject go_seg1;
    [SerializeField] GameObject go_seg2;
    [SerializeField] GameObject go_seg3;
    [SerializeField] GameObject go_seg4;
    [SerializeField] GameObject go_seg5;
    [SerializeField] GameObject go_seg6;
    [SerializeField] GameObject go_seg7;
    [SerializeField] GameObject go_seg8;
    [SerializeField] GameObject go_seg9;
    [SerializeField] GameObject go_seg10;
    [SerializeField] TMP_Text tmp_skipText;
    [SerializeField] PlayerCompass playerCompass;
    [SerializeField] GameObject go_playerCompass;

    // game objects for compass targets
    [SerializeField] GameObject go_diningSwitch;
    [SerializeField] GameObject go_map;
    [SerializeField] GameObject go_dish;
    [SerializeField] GameObject go_door;

    private void Awake()
    {
        GameManager.menuManager.StartScreenClosed += StartTutorial;
        LightSwitch.AnySwitchToggled += UpdateLightSwitchToggled;
    }

    private void Start()
    {
        bl_tutorialActive = false;
        int_tutorialSegment = 0;
    }

    void Update()
    {
        // if in the tutorial and player can move
        if (!bl_tutorialActive || GameManager.playerController.En_state != PlayerController.State.active) return;
        
        // skip to final segment if enter is pressed
        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyUp(KeyCode.KeypadEnter))
        {
            int_tutorialSegment = 11;
            GameManager.taskManager.SetCurrentChore(1);
        }

        // perform action based on current segment, then go to next segment if completed
        switch (int_tutorialSegment)
        {
            case 1:
                // check if mouse has moved
                if (v3_prevMousePos != Input.mousePosition)
                    int_mousePosChanges++;
                if (int_mousePosChanges >= 3)
                {
                    int_tutorialSegment++;
                    go_seg1.SetActive(false);
                    go_seg2.SetActive(true);
                }
                v3_prevMousePos = Input.mousePosition;
                break;
            case 2:
                // check if player is pressing WASD
                if (Input.GetKey(KeyCode.W) ||
                    Input.GetKey(KeyCode.A) ||
                    Input.GetKey(KeyCode.S) ||
                    Input.GetKey(KeyCode.D))
                {
                    int_tutorialSegment++;
                    go_seg2.SetActive(false);
                    go_seg3.SetActive(true);
                }
                break;
            case 3:
                // check when player presses space
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    int_tutorialSegment++;
                    go_seg3.SetActive(false);
                    go_seg4.SetActive(true);
                }
                break;
            case 4:
                // check if player is pressing shift
                if (Input.GetKey(KeyCode.LeftShift) ||
                    Input.GetKey(KeyCode.RightShift))
                {
                    int_tutorialSegment++;
                    go_seg4.SetActive(false);
                    go_seg5.SetActive(true);
                    go_playerCompass.SetActive(true);
                    playerCompass.SetTarget(go_map);
                }
                break;
            case 5:
                // check if player is holding the map
                if (GameManager.playerController.Go_heldObject != null &&
                    GameManager.playerController.Go_heldObject.name == "map")
                {
                    int_tutorialSegment++;
                    go_seg5.SetActive(false);
                    go_seg6.SetActive(true);
                    go_playerCompass.SetActive(false);
                }
                break;
            case 6:
                // check if player is scrolling while holding an object
                if (Input.GetAxisRaw("Mouse ScrollWheel") != 0 &&
                    GameManager.playerController.Go_heldObject != null)
                {
                    int_tutorialSegment++;
                    go_seg6.SetActive(false);
                    go_seg7.SetActive(true);
                }
                break;
            case 7:
                // check if player is not holding anything
                if (GameManager.playerController.Go_heldObject == null)
                {
                    int_tutorialSegment++;
                    go_seg7.SetActive(false);
                    go_seg8.SetActive(true);
                    go_playerCompass.SetActive(true);
                    playerCompass.SetTarget(go_diningSwitch);
                }
                break;
            case 8:
                // check if any light switch is on
                if (bl_hasASwitchChanged)
                {
                    int_tutorialSegment++;
                    go_seg8.SetActive(false);
                    go_seg9.SetActive(true);
                    playerCompass.SetTarget(go_door);
                }
                break;
            case 9:
                // check if player has passed through the kitchen door
                if (doorZone.bl_playerIn)
                {
                    int_tutorialSegment++;
                    go_seg9.SetActive(false);
                    go_seg10.SetActive(true);
                    playerCompass.SetTarget(go_dish);

                    GameManager.taskManager.SetCurrentChore(1);
                }
                break;
            case 10:
                // check if player is at the kitchen island
                if (dishZone.bl_playerIn)
                    int_tutorialSegment++;
                break;
            case 11:
                EndTutorial();
                break;
        }
    }

    // unsubscribe from event when destroyed
    private void OnDestroy()
    {
        GameManager.menuManager.StartScreenClosed -= StartTutorial;
        LightSwitch.AnySwitchToggled -= UpdateLightSwitchToggled;
    }

    // begin tutorial sequence
    void StartTutorial(object source, EventArgs e)
    {
        bl_tutorialActive = true;
        int_tutorialSegment = 1;

        go_seg1.SetActive(true);
        tmp_skipText.gameObject.SetActive(true);

        v3_prevMousePos = Input.mousePosition;
        int_mousePosChanges = 0;
    }

    // disable all tutorial UI
    void EndTutorial()
    {
        bl_tutorialActive = false;
        int_tutorialSegment = 0;

        go_seg1.SetActive(false);
        go_seg2.SetActive(false);
        go_seg3.SetActive(false);
        go_seg4.SetActive(false);
        go_seg5.SetActive(false);
        go_seg6.SetActive(false);
        go_seg7.SetActive(false);
        go_seg8.SetActive(false);
        go_seg9.SetActive(false);
        go_seg10.SetActive(false);
        tmp_skipText.gameObject.SetActive(false);
        go_playerCompass.SetActive(false);
    }

    // called when triggered by light switch toggle event
    void UpdateLightSwitchToggled(object source, EventArgs e)
    {
        bl_hasASwitchChanged = true;
    }
}