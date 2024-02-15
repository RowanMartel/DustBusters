using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toy : Pickupable
{
    [HideInInspector]public bool bl_inBox;

    private void Start()
    {
        bl_pickupable = true;
    }
}
