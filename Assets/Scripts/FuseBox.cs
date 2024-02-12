using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBox : Interactable
{
    public bool bl_isOn = true;
    protected bool bl_ready = true;

    public GameObject go_lever;

    LightSwitch[] a_ls_switches;
    TVStatic tv_static;

    int int_timesToFlicker;
    public float flt_timeTweenFlicker;
    float flt_curFlickerTime;
    [Tooltip("The time between each time the ghost can cause the power to flicker")]
    public float flt_flickerDelay;
    float flt_curFlickerDelay;
    public int int_maxFlickerTimes;
    public int int_minFlickerTimes;

    // Start is called before the first frame update
    void Start()
    {
        a_ls_switches = FindObjectsByType<LightSwitch>(FindObjectsSortMode.None);
        tv_static = FindObjectOfType<TVStatic>();
        SetSwitchesOn();
        flt_curFlickerTime = 0;
        int_timesToFlicker = 0;
        flt_curFlickerDelay = 0;
    }

    public void Update()
    {
        if(flt_curFlickerDelay > 0)
        {
            flt_curFlickerDelay -= Time.deltaTime;
        }

        if(int_timesToFlicker > 0)
        {
            flt_curFlickerTime -= Time.deltaTime;
            if(flt_curFlickerTime <= 0)
            {
                flt_curFlickerTime = flt_timeTweenFlicker;
                if (bl_isOn)
                {
                    bl_isOn = false;
                    SetSwitchesOff();
                }
                else
                {
                    bl_isOn = true;
                    SetSwitchesOn();
                    int_timesToFlicker--;
                }
            }
        }
    }

    public override void Interact()
    {
        //Pulls the lever and tells the switches and task manager know the state
        if(bl_isOn && bl_ready)
        {
            bl_ready = false;
            LeanTween.rotateLocal(go_lever, new Vector3(90, 0, 0), 0.8f).setEase(LeanTweenType.easeInQuint).setOnComplete(SetSwitchesOff);
            bl_isOn = false;
            GameManager.taskManager.AddTask(TaskManager.Task.ResetBreakerBox);
        }
        else if(!bl_isOn && bl_ready)
        {
            bl_ready = false;
            LeanTween.rotateLocal(go_lever, new Vector3(0, 0, 0), 0.8f).setEase(LeanTweenType.easeInQuint).setOnComplete(SetSwitchesOn);
            bl_isOn = true;
            GameManager.taskManager.CompleteTask(TaskManager.Task.ResetBreakerBox);
        }
    }

    //Lets all switches know they should have power
    void SetSwitchesOn()
    {
        foreach (LightSwitch ls_switch in a_ls_switches)
        {
            ls_switch.SetFuseActive(true);
        }
        tv_static.bl_powered = true;
        tv_static.Refresh();
        bl_ready = true;
    }

    //Lets all switches know they shouldn't have power
    void SetSwitchesOff()
    {
        foreach (LightSwitch ls_switch in a_ls_switches)
        {
            ls_switch.SetFuseActive(false);
        }
        tv_static.bl_powered = false;
        tv_static.Refresh();
        bl_ready = true;
    }

    public void Flicker()
    {
        if (flt_curFlickerDelay > 0) return;
        int_timesToFlicker = Random.Range(int_minFlickerTimes, int_maxFlickerTimes);
        flt_curFlickerTime = flt_timeTweenFlicker;
        flt_curFlickerDelay = flt_flickerDelay;
        bl_isOn = false;
        SetSwitchesOff();
    }

}
