using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pickupable : Interactable
{
    [Tooltip("Check if object can be used to mop the floor")]
    public bool bl_mop;

    [Tooltip("Check if object can be used to sweep cobwebs")]
    public bool bl_duster;

    [Tooltip("check if object can be used to clean the mirror")]
    public bool bl_soapBar;

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

    [Tooltip("check if the damage SFX from this object should be blunt or sharp")]
    public bool bl_sharp;

    [Tooltip("Rotation applied when held")]
    public Vector3 v3_heldRotationMod;

    [Tooltip("Position applied when held")]
    public Vector3 v3_heldPositionMod;

    protected Rigidbody rb;
    public Rigidbody RB { get { return rb; } }
    protected Collider[] l_col;
    public Collider[] l_Col { get { return l_col; } }
    [HideInInspector] public bool bl_held;
    protected MeshRenderer ren_meshRenderer;
    // holds the default material for objects that change materials
    // i.e. clean material for dirty dish
    protected Material mat_base;

    List<Collider> l_col_overlapping = new List<Collider>();

    protected virtual void Start()
    {
        bl_pickupable = true;
        ren_meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        l_col = GetComponents<Collider>();
        bl_held = false;
        mat_base = ren_meshRenderer.material;
    }

    //Trigger Enter/Exit scripts are used to make sure objects don't stuck in the environment when picked up
    private void OnTriggerEnter(Collider other)
    {
        if (!l_Col[0].isTrigger)return;
        if(other.isTrigger || l_Col.Contains(other) || l_col_overlapping.Contains(other)) return;
        l_col_overlapping.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        l_col_overlapping.Remove(other);
        if(l_col_overlapping.Count <= 0)
        {
            foreach(Collider co in l_col)
            {
                co.isTrigger = false;
            }
        }
    }

    public void Drop()
    {
        foreach (Collider co in l_col)
        {
            co.isTrigger = false;
        }
    }

    // turns collider off when picked up, until item is in hand. This should prevent things from getting stuck in hand.
    public override void Interact()
    {
        if(transform.GetComponent<Candle>())
        {
            if (GameManager.playerController.Go_heldObject.GetComponent<Pickupable>().bl_lighter) transform.GetComponent<Candle>().Light();
        }

        l_col_overlapping.Clear();
        foreach (Collider co in l_col)
        {
            co.isTrigger = true;
        }
    }
}