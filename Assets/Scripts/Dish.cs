using UnityEngine;

public class Dish : Pickupable
{
    [HideInInspector] public bool inCupboard;
    [HideInInspector] public bool inTrash;

    [Tooltip("Check if the dish starts broken")]
    public bool broken;

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
        audioSource = GetComponent<AudioSource>();

        baseMat = meshRenderer.material;

        if (dirtyDish)
            meshRenderer.material = dirtyMat;

        if (broken) Break();
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
        audioSource.PlayOneShot(breakingSFX);
        GetComponent<MeshFilter>().mesh = brokenMesh;

        CleaningWater cleaningWater = FindObjectOfType<CleaningWater>();
        CupboardTrigger cupboardTrigger = FindObjectOfType<CupboardTrigger>();
        TrashCanTrigger trashCanTrigger = FindObjectOfType<TrashCanTrigger>();

        cleaningWater.dishes.Remove(this);
        cupboardTrigger.dishes.Remove(this);
        trashCanTrigger.dishes.Add(this);

        //GameManager.ghost?.patrolPointsPerTask[GameManager.ghost.masterTaskList.IndexOf(TaskManager.Task.PutAwayDishes)].list.Remove(transform);
        //GameManager.ghost?.currentPoints[GameManager.ghost.currentTasks.IndexOf(TaskManager.Task.PutAwayDishes)].list.Remove(transform);

        cupboardTrigger.CheckIfComplete();
    }

    // marks dish as clean and changes back to the clean material
    public void Clean()
    {
        if (!dirtyDish) return;
        dirtyDish = false;
        meshRenderer.material = baseMat;
    }
}