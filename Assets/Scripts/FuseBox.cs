using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBox : Interactable
{
    public bool bl_isOn = true;
    protected bool bl_ready = true;

    public GameObject go_lever;

    LightSwitch[] a_ls_switches;
    TVStatic[] a_tv_static;

    int int_timesToFlicker;
    public float flt_timeTweenFlicker;
    float flt_curFlickerTime;
    [Tooltip("The time between each time the ghost can cause the power to flicker")]
    public float flt_flickerDelay;
    float flt_curFlickerDelay;
    public int int_maxFlickerTimes;
    public int int_minFlickerTimes;
    public AudioSource as_source;
    public AudioSource as_loop;
    public AudioClip ac_activate;
    public AudioClip ac_deactivate;

    // Start is called before the first frame update
    void Start()
    {
        a_ls_switches = FindObjectsByType<LightSwitch>(FindObjectsSortMode.None);
        a_tv_static = FindObjectsByType<TVStatic>(FindObjectsSortMode.None);
        SetSwitchesOn();
        flt_curFlickerTime = 0;
        int_timesToFlicker = 0;
        flt_curFlickerDelay = 0;
    }

    public void Update()
    {
        //Perform Flicker when appropriate
        if(int_timesToFlicker > 0)
        {
            flt_curFlickerTime -= Time.deltaTime;
            if(flt_curFlickerTime <= 0)
            {
                flt_curFlickerTime = flt_timeTweenFlicker;
                if (bl_isOn)
                {
                    //Turn power off
                    bl_isOn = false;
                    SetSwitchesOff();
                }
                else
                {
                    //Turn power on
                    bl_isOn = true;
                    int_timesToFlicker--;
                    SetSwitchesOn();
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
            LeanTween.rotateLocal(go_lever, new Vector3(90, 0, 0), 0.4f).setEase(LeanTweenType.easeInCubic).setOnComplete(SetSwitchesOff);
            bl_isOn = false;
            GameManager.taskManager.AddTask(TaskManager.Task.ResetBreakerBox);
        }
        else if(!bl_isOn && bl_ready)
        {
            bl_ready = false;
            LeanTween.rotateLocal(go_lever, new Vector3(0, 0, 0), 0.4f).setEase(LeanTweenType.easeOutCubic);
            bl_isOn = true;
            GameManager.taskManager.CompleteTask(TaskManager.Task.ResetBreakerBox);
            SetSwitchesOn();
        }
    }

    //Lets all switches know they should have power
    void SetSwitchesOn()
    {
        Debug.Log("Switches ON");

        GameManager.soundManager.PlayClip(ac_activate, as_source, true);
        as_loop.Play();
        foreach (LightSwitch ls_switch in a_ls_switches)
        {
            ls_switch.SetFuseActive(true);
        }
        foreach(TVStatic tv in a_tv_static)
        {
            tv.bl_powered = true;
            tv.Refresh();
        }
        bl_ready = true;
    }

    //Lets all switches know they shouldn't have power
    void SetSwitchesOff()
    {
        Debug.Log("Switches OFF");

        GameManager.soundManager.PlayClip(ac_deactivate, as_source, true);
        as_loop.Stop();
        foreach (LightSwitch ls_switch in a_ls_switches)
        {
            ls_switch.SetFuseActive(false);
        }
        foreach (TVStatic tv in a_tv_static)
        {
            tv.bl_powered = false;
            tv.Refresh();
        }
        bl_ready = true;
    }

    //Start Flicker
    public void Flicker()
    {
        if (flt_curFlickerDelay > 0 || !bl_isOn) return;
        int_timesToFlicker = Random.Range(int_minFlickerTimes, int_maxFlickerTimes);
        flt_curFlickerTime = flt_timeTweenFlicker;
        flt_curFlickerDelay = flt_flickerDelay;
        bl_isOn = false;
        SetSwitchesOff();
    }

}
