using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dish : Pickupable
{
    [HideInInspector] public bool inCupboard;
    [HideInInspector] public bool inTrash;

    [Tooltip("Check if the dish starts broken")]
    public bool bl_broken;

    [Tooltip("Add broken mesh here")]
    public Mesh brokenMesh;

    [Tooltip("Check if object can be cleaned by placing it in the sink")]
    public bool bl_dirtyDish;

    [Tooltip("Add material here if dirtyDish is true")]
    public Material mat_dirtyDish;

    [Tooltip("Put the breaking SFX here")]
    public AudioClip ac_break;
    AudioSource as_source;

    protected override void Start()
    {
        base.Start();

        bl_pickupable = true;
        ren_meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        as_source = GetComponent<AudioSource>();

        mat_base = ren_meshRenderer.material;

        if (bl_dirtyDish)
            ren_meshRenderer.material = mat_dirtyDish;

        if (bl_broken) Break();
    }

    // calls break if the dish collides with anything too hard
    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 10)
            Break();
    }

    // changes the model and adjusts all tasks relating to this dish to either remove or add it as a requirement
    void Break()
    {
        GameManager.soundManager.PlayClip(ac_break, as_source);
        GetComponent<MeshFilter>().mesh = brokenMesh;
        bl_broken = true;

        List<CleaningWater> waters = FindObjectsByType<CleaningWater>(FindObjectsSortMode.None).ToList();
        List<CupboardTrigger> cupboards = FindObjectsByType<CupboardTrigger>(FindObjectsSortMode.None).ToList();
        List<TrashCanTrigger> trashes = FindObjectsByType<TrashCanTrigger>(FindObjectsSortMode.None).ToList();

        foreach(CleaningWater w in waters)
        {
            w.li_dishes.Remove(this);
            w.CheckIfComplete();
        }
        foreach(CupboardTrigger c in cupboards)
        {
            c.li_dishes.Remove(this);
            c.CheckIfComplete();
        }
        foreach(TrashCanTrigger t in trashes)
        {
            t.li_dishes.Add(this);
            t.CheckIfComplete();
        }

        GameManager.ghost.RemovePoint(transform);
    }

    // marks dish as clean and changes back to the clean material
    public void Clean()
    {
        if (!bl_dirtyDish) return;
        bl_dirtyDish = false;
        ren_meshRenderer.material = mat_base;
    }
}