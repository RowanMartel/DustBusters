using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [HideInInspector] public bool pickupable;

    public abstract void Interact();

    private void Awake()
    {
        GetComponent<Outline>().enabled = false;
    }
}