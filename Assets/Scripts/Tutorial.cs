using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    int int_tutorialSegment;
    bool bl_tutorialActive;
    [SerializeField] TMP_Text tmp_skipText;

    // variables for segment completion detection
    Vector3 v3_prevMousePos;
    [SerializeField] LightSwitch diningSwitch;
    [SerializeField] PlayerCheckZone doorZone;
    [SerializeField] PlayerCheckZone dishZone;

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

    private void Awake()
    {
        GameManager.menuManager.StartScreenClosed += StartTutorial;
    }

    private void Start()
    {
        bl_tutorialActive = false;
        int_tutorialSegment = 0;
    }

    void Update()
    {
        if (bl_tutorialActive)
        {
            if (Input.GetKeyDown(KeyCode.Return) ||
                Input.GetKeyUp(KeyCode.KeypadEnter))
                int_tutorialSegment = 10;

            switch (int_tutorialSegment)
            {
                case 1:
                    if (v3_prevMousePos != Input.mousePosition)
                    {
                        int_tutorialSegment++;
                        go_seg1.SetActive(false);
                        go_seg2.SetActive(true);
                    }
                    v3_prevMousePos = Input.mousePosition;
                    break;
                case 2:
                    if (Input.GetKeyDown(KeyCode.W) ||
                        Input.GetKeyDown(KeyCode.A) ||
                        Input.GetKeyDown(KeyCode.S) ||
                        Input.GetKeyDown(KeyCode.D))
                    {
                        int_tutorialSegment++;
                        go_seg2.SetActive(false);
                        go_seg3.SetActive(true);
                    }
                    break;
                case 3:
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        int_tutorialSegment++;
                        go_seg3.SetActive(false);
                        go_seg4.SetActive(true);
                    }
                    break;
                case 4:
                    if (Input.GetKeyDown(KeyCode.LeftShift) ||
                        Input.GetKeyDown(KeyCode.RightShift))
                    {
                        int_tutorialSegment++;
                        go_seg4.SetActive(false);
                        go_seg5.SetActive(true);
                    }
                    break;
                case 5:
                    if (GameManager.playerController.Go_heldObject.name == "map")
                    {
                        int_tutorialSegment++;
                        go_seg5.SetActive(false);
                        go_seg6.SetActive(true);
                    }
                    break;
                case 6:
                    if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
                    {
                        int_tutorialSegment++;
                        go_seg6.SetActive(false);
                        go_seg7.SetActive(true);
                    }
                    break;
                case 7:
                    if (diningSwitch.bl_on)
                    {
                        int_tutorialSegment++;
                        go_seg7.SetActive(false);
                        go_seg8.SetActive(true);
                    }
                    break;
                case 8:
                    if (doorZone.bl_playerIn)
                    {
                        int_tutorialSegment++;
                        go_seg8.SetActive(false);
                        go_seg9.SetActive(true);
                    }
                    break;
                case 9:
                    if (dishZone.bl_playerIn)
                    {
                        int_tutorialSegment++;
                        go_seg9.SetActive(false);
                    }
                    break;
                case 10:
                    EndTutorial();
                    break;
            }
        }
    }

    void StartTutorial(object source, EventArgs e)
    {
        bl_tutorialActive = true;
        int_tutorialSegment = 1;

        go_seg1.SetActive(true);
        tmp_skipText.enabled = true;

        v3_prevMousePos = Input.mousePosition;
    }
    void EndTutorial()
    {
        tmp_skipText.enabled = false;
        bl_tutorialActive = false;
        int_tutorialSegment = 0;
    }
}