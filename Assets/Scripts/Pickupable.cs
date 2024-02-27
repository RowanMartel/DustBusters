using UnityEngine;

public class Pickupable : Interactable
{
    [Tooltip("Check if object can be used to mop the floor")]
    public bool bl_mop;

    [Tooltip("Check if object can be used to sweep cobwebs and clean the mirror")]
    public bool bl_duster;

    [Tooltip("Check if object can be used to light the fireplace")]
    public bool bl_lighter;

    [Tooltip("Check if object can be used to unlock the front door")]
    public bool bl_frontDoorKey;

    [Tooltip("Check if object can be used to turn off the TV")]
    public bool bl_remote;

    [Tooltip("Check if object is a doorknob")]
    public bool bl_doorKnob;

    [Tooltip("Check if the ghost wants to hide this")]
    public bool bl_hideable;

    [Tooltip("Check if the ghost wants to throw this")]
    public bool bl_toThrow;

    [Tooltip("Check if this object can damage the player")]
    public bool bl_canDamagePlayer;

    [Tooltip("Rotation applied when held")]
    public Vector3 v3_heldRotationMod;

    [Tooltip("Position applied when held")]
    public Vector3 v3_heldPositionMod;

    protected Rigidbody rb;
    public Rigidbody RB { get { return rb; } }
    [HideInInspector] public bool bl_held;
    protected MeshRenderer ren_meshRenderer;
    // holds the default material for objects that change materials
    // i.e. clean material for dirty dish
    protected Material mat_base;

    protected virtual void Start()
    {
        bl_pickupable = true;
        ren_meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        bl_held = false;
        mat_base = ren_meshRenderer.material;
    }

    // turns collider off when picked up, until item is in hand. This should prevent things from getting stuck in hand.
    public override void Interact()
    {
    }
}