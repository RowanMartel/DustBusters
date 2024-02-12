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

    [Tooltip("Check if the ghost wants to hide this")]
    public bool bl_hideable;

    [Tooltip("Check if the ghost wants to break this")]
    public bool bl_breakable;

    [Tooltip("Check if this object can damage the player")]
    public bool bl_canDamagePlayer;

    [Tooltip("Rotation applied when held")]
    public Vector3 v3_heldRotationMod;

    [Tooltip("Position applied when held")]
    public Vector3 v3_heldPositionMod;

    protected Rigidbody rb;
    [HideInInspector] public bool bl_held;
    protected MeshRenderer ren_meshRenderer;
    // holds the default material for objects that change materials
    // i.e. clean material for dirty dish
    protected Material mat_base;

    private void Start()
    {
        bl_pickupable = true;
        ren_meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        mat_base = ren_meshRenderer.material;
    }

    // does nothing
    public override void Interact(){}
}