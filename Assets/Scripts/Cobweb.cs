using UnityEngine;

public class Cobweb : MonoBehaviour
{
    [HideInInspector]
    public bool bl_cleaned;
    Cobwebs cobwebs;
    MeshRenderer ren;

    [Tooltip("Put the dusting SFX here")]
    public AudioClip ac_clean;
    AudioSource as_source;

    public GameObject go_dustParticles;
    public Color clr_dustColor;

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

        GameManager.soundManager.PlayClip(ac_clean, as_source);

        //Dust particle effect
        GameObject go_dust = Instantiate(go_dustParticles, transform);
        go_dust.GetComponent<ParticleSystem>().startColor = clr_dustColor;

        ren.enabled = false;
        bl_cleaned = true;
        cobwebs.CleanWeb();
    }
}