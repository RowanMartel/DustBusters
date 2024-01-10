using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dish : Pickupable
{
    [HideInInspector] public bool inCupboard;
    [HideInInspector] public bool inTrash;
    [HideInInspector] public bool broken;

    [Tooltip("Add broken mesh here")]
    public Mesh brokenMesh;

    [Tooltip("Check if object can be cleaned by placing it in the sink")]
    public bool dirtyDish;

    [Tooltip("Add material here if dirtyDish is true")]
    public Material dirtyMat;

    [Tooltip("Put the breaking SFX here")]
    public AudioClip breakingSFX;
    AudioSource audioSource;

    private void Start()
    {
        pickupable = true;
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();

        baseMat = meshRenderer.material;

        if (dirtyDish)
            meshRenderer.material = dirtyMat;

        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 10)
            Break();
    }

    void Break()
    {
        audioSource.PlayOneShot(breakingSFX);
        GetComponent<MeshFilter>().mesh = brokenMesh;

        CleaningWater cleaningWater = FindObjectOfType<CleaningWater>();
        CupboardTrigger cupboardTrigger = FindObjectOfType<CupboardTrigger>();
        TrashCanTrigger trashCanTrigger = FindObjectOfType<TrashCanTrigger>();

        cleaningWater.dishes.Remove(this);
        cupboardTrigger.dishes.Remove(this);
        trashCanTrigger.dishes.Add(this);

        cupboardTrigger.CheckIfComplete();
    }

    public void Clean()
    {
        if (!dirtyDish) return;
        dirtyDish = false;
        meshRenderer.material = baseMat;
    }
}