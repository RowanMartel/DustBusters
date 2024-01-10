using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Cobweb : MonoBehaviour
{
    [HideInInspector]
    public bool cleaned;
    int dirtLevel = 5;
    Material material;
    Cobwebs cobwebs;

    private void Start()
    {
        cobwebs = GetComponentInParent<Cobwebs>();
        material = GetComponent<Renderer>().material;
    }

    private void OnTriggerExit(Collider other)
    {
        Pickupable pickupable = other.GetComponent<Pickupable>();
        if (!pickupable || !pickupable.duster || cleaned) return;

        dirtLevel--;
        material.color = new Color(material.color.r, material.color.g, material.color.g, material.color.a - .2f);
        if (dirtLevel == 0)
        {
            cleaned = true;
            cobwebs.CleanWeb();
        }
    }
}