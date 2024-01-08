using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : Interactable
{
    [Tooltip("Check if object can be cleaned by placing it in the sink")]
    public bool dirtyDish;

    [Tooltip("Check if object must be placed in cupboard for putting-away-dishes task")]
    public bool dish;

    [Tooltip("Check if object can be used to mop the floor")]
    public bool mop;

    [Tooltip("Check if object can be used to sweep cobwebs")]
    public bool duster;

    [Tooltip("Check if object can be used to light the fireplace")]
    public bool lighter;

    [Tooltip("Check if object can be used to clean the mirror")]
    public bool mirrorCleaner;

    [Tooltip("Check if object can be used to unlock the front door")]
    public bool frontDoorKey;

    [Tooltip("Add material here if dirtyDish is true")]
    public Material dirtyMat;

    Rigidbody rb;
    [HideInInspector] public bool held;
    Material baseMat;
    MeshRenderer meshRenderer;
    [HideInInspector] public bool inCupboard;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();

        baseMat = meshRenderer.material;

        if (dirtyDish)
            meshRenderer.material = dirtyMat;
    }

    public override void Interact()
    {
        throw new System.NotImplementedException();
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

    public void Clean()
    {
        if (!dirtyDish) return;
        dirtyDish = false;
        meshRenderer.material = baseMat;
    }
}