using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerContents : MonoBehaviour
{
    //If the player adds or removes an object from a drawer, this will parent/unparent it
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Interactable") return;

        other.transform.parent = transform.parent;
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.parent = null;
    }
}
