using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerOpen : Interactable
{
    public enum Direction
    {
        xUP,
        xDOWN,
        zUP,
        zDOWN
    }

    public Direction en_openDirection;
    public bool bl_open;

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
        Toggle();
    }

    void Toggle()
    {

    }
}
