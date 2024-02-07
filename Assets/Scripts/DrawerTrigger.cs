using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerContents : MonoBehaviour
{
    // Start is called before the first frame update
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
