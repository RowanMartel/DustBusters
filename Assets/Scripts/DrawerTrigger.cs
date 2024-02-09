using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerContents : MonoBehaviour
{
    public List<GameObject> l_go_curContents = new List<GameObject>();

    //If the player adds or removes an object from a drawer, this will parent/unparent it
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Interactable") return;

        other.transform.parent = transform.parent;
        l_go_curContents.Add(other.gameObject);
    }

    //This was causing issues with the Float Trigger
    private void OnTriggerExit(Collider other)
    {
        if (!l_go_curContents.Contains(other.gameObject)) return;

        other.transform.parent = null;
    }
}
