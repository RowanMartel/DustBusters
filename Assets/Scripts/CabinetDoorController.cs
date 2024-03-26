using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

public class CabinetDoorController : Interactable
{
    public enum Side
    {
        right,
        left
    }

    public Side side;
    public float fl_openAmount;
    protected bool bl_open = false;
    protected bool bl_ready = true;
    public bool Bl_ready { get { return bl_ready; } }

    protected Vector3 v3_openRotation;
    protected Vector3 v3_closedRotation;

    // Start is called before the first frame update
    private void Start()
    {
        v3_closedRotation = transform.parent.localEulerAngles;
        if(side == Side.left) v3_openRotation = new Vector3(v3_closedRotation.x, v3_closedRotation.y + 80, v3_closedRotation.z);
        else if (side == Side.right) v3_openRotation = new Vector3(v3_closedRotation.x, v3_closedRotation.y - 80, v3_closedRotation.z);

    }


    public override void Interact()
    {
        Toggle();
    }

    void Toggle()
    {
        //Open door
        if (!bl_open && bl_ready)
        {
            bl_ready = false;
            LeanTween.rotateLocal(transform.parent.gameObject, v3_openRotation, 1).setEase(LeanTweenType.easeOutSine).setOnComplete(DoorReady);
            bl_open = true;
        }

        //Close door
        if (bl_open && bl_ready)
        {
            bl_ready = false;
            LeanTween.rotateLocal(transform.parent.gameObject, v3_closedRotation, 1).setEase(LeanTweenType.easeOutSine).setOnComplete(DoorReady);
            bl_open = false;
        }
    }

    void DoorReady()
    {
        bl_ready = true;
    }
}
