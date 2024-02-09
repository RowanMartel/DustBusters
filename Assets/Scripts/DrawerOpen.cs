using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

public class DrawerOpen : Interactable
{
    public enum Direction
    {
        xPlus,
        xMinus,
        zPlus,
        zMinus
    }

    public Direction en_openDirection;
    public float fl_openAmount;
    protected bool bl_open = false;
    protected bool bl_ready = true;

    protected Vector3 v3_openPosition;
    protected Vector3 v3_closedPosition;

    // Start is called before the first frame update
    private void Start()
    {
        v3_closedPosition = transform.localPosition;

        // We can tell any drawer instance which direction to open using the Direction enum and setting it in the Unity inspector
        switch (en_openDirection)
        {
            case Direction.xPlus:
                v3_openPosition = new Vector3(v3_closedPosition.x + fl_openAmount, v3_closedPosition.y, v3_closedPosition.z);
                break;

            case Direction.xMinus:
                v3_openPosition = new Vector3(v3_closedPosition.x - fl_openAmount, v3_closedPosition.y, v3_closedPosition.z);
                break;

            case Direction.zPlus:
                v3_openPosition = new Vector3(v3_closedPosition.x, v3_closedPosition.y, v3_closedPosition.z + fl_openAmount);
                break;

            case Direction.zMinus:
                v3_openPosition = new Vector3(v3_closedPosition.x, v3_closedPosition.y, v3_closedPosition.z - fl_openAmount);
                break;
        }
    }


    public override void Interact()
    {
        Toggle();
    }

    void Toggle()
    {
        //Open drawer
        if (!bl_open && bl_ready)
        {

            bl_ready = false;
            LeanTween.moveLocal(transform.gameObject, v3_openPosition, 1).setEase(LeanTweenType.easeOutSine).setOnComplete(DrawerReady);
            bl_open = true;
        }

        //Close drawer
        if (bl_open && bl_ready)
        {
            bl_ready = false;
            LeanTween.moveLocal(transform.gameObject, v3_closedPosition, 1).setEase(LeanTweenType.easeOutSine).setOnComplete(DrawerReady);
            bl_open = false;
        }
    }

    void DrawerReady()
    {
        bl_ready = true;
    }
}
