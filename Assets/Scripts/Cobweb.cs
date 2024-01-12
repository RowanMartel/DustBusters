using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cobweb : MonoBehaviour
{
    [HideInInspector]
    public bool cleaned;
    Cobwebs cobwebs;
    MeshRenderer meshRenderer;

    [Tooltip("Put the dusting SFX here")]
    public AudioClip cleanSFX;
    AudioSource audioSource;

    private void Start()
    {
        cobwebs = GetComponentInParent<Cobwebs>();
        meshRenderer = GetComponent<MeshRenderer>();

        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Pickupable pickupable = other.GetComponent<Pickupable>();
        if (!pickupable || !pickupable.duster || cleaned) return;

        audioSource.PlayOneShot(cleanSFX);
        meshRenderer.enabled = false;
        cleaned = true;
        cobwebs.CleanWeb();
    }
}