using UnityEngine;

public class Cobweb : MonoBehaviour
{
    [HideInInspector]
    public bool bl_cleaned;
    Cobwebs cobwebs;
    MeshRenderer ren;

    [Tooltip("Put the dusting SFX here")]
    public AudioClip cl_clean;
    AudioSource as_source;

    private void Start()
    {
        cobwebs = GetComponentInParent<Cobwebs>();
        ren = GetComponent<MeshRenderer>();
        as_source = GetComponent<AudioSource>();
    }

    // cleans the cobweb when colliding with the duster
    private void OnTriggerEnter(Collider other)
    {
        Pickupable pickupable = other.GetComponent<Pickupable>();
        if (!pickupable || !pickupable.bl_duster || bl_cleaned) return;

        GameManager.soundManager.PlayClip(cl_clean, as_source);
        ren.enabled = false;
        bl_cleaned = true;
        cobwebs.CleanWeb();
    }
}