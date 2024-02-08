using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseBox : Interactable
{
    protected bool bl_isOn = true;
    protected bool bl_ready = true;

    public GameObject go_lever;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Interact()
    {
        if(bl_isOn && bl_ready)
        {
            bl_ready = false;
            LeanTween.rotateLocal(go_lever, new Vector3(90, 0, 0), 0.8f).setEase(LeanTweenType.easeInQuint).setOnComplete(FuseBoxReady);
            bl_isOn = false;
        }
        else if(!bl_isOn && bl_ready)
        {
            bl_ready = false;
            LeanTween.rotateLocal(go_lever, new Vector3(0, 0, 0), 0.8f).setEase(LeanTweenType.easeInQuint).setOnComplete(FuseBoxReady);
            bl_isOn = true;
        }
    }

    protected void FuseBoxReady()
    {
        bl_ready = true;
        if (bl_isOn) Debug.Log("Fuse Box ON"); // this is where we probably want to interact with the Task Manager, and turn on all active lights
    }
}
