using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : Interactable
{
    [Tooltip("Check if object can be used to mop the floor")]
    public bool mop;

    [Tooltip("Check if object can be used to sweep cobwebs and clean the mirror")]
    public bool duster;

    [Tooltip("Check if object can be used to light the fireplace")]
    public bool lighter;

    [Tooltip("Check if object can be used to unlock the front door")]
    public bool frontDoorKey;

    protected Rigidbody rb;
    [HideInInspector] public bool held;
    protected Material baseMat;
    protected MeshRenderer meshRenderer;

    private void Start()
    {
        pickupable = true;
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();

        baseMat = meshRenderer.material;
    }

    public override void Interact()
    {
        // do nothing
    }

    public void PickUp()
    {
        rb.isKinematic = true;
        held = true;
    }
    public void Drop()
    {
        rb.isKinematic = false;
        held = false;
    }
}