using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [HideInInspector] public bool pickupable;

    public virtual void Interact()
    {
        Debug.Log("interacted with a thing");
    }

    private void Awake()
    {
        GetComponent<Outline>().enabled = false;
    }
}