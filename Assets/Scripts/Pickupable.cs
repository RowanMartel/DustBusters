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

    [Tooltip("Check if the ghost wants to hide this")]
    public bool hideable;

    [Tooltip("Check if the ghost wants to break this")]
    public bool breakable;

    [Tooltip("Check if this object can damage the player")]
    public bool canDamagePlayer;

    [Tooltip("Rotation applied when held")]
    public Vector3 heldRotationMod;

    protected Rigidbody rb;
    [HideInInspector] public bool held;
    protected MeshRenderer meshRenderer;
    // holds the default material for objects that change materials
    // i.e. clean material for dirty dish
    protected Material baseMat;

    private void Start()
    {
        pickupable = true;
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        baseMat = meshRenderer.material;
    }

    // does nothing
    public override void Interact(){}

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