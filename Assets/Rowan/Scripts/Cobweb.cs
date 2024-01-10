using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Cobweb : MonoBehaviour
{
    [HideInInspector]
    public bool cleaned;
    int dirtLevel = 5;
    Material material;
    Cobwebs cobwebs;
    MeshRenderer meshRenderer;

    [Tooltip("Put the dusting SFX here")]
    public AudioClip cleanSFX;
    AudioSource audioSource;

    private void Start()
    {
        cobwebs = GetComponentInParent<Cobwebs>();
        material = GetComponent<Renderer>().material;
        meshRenderer = GetComponent<MeshRenderer>();

        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerExit(Collider other)
    {
        Pickupable pickupable = other.GetComponent<Pickupable>();
        if (!pickupable || !pickupable.duster || cleaned) return;

        dirtLevel--;
        material.color = new Color(material.color.r, material.color.g, material.color.g, material.color.a - .2f);
        audioSource.PlayOneShot(cleanSFX);
        if (dirtLevel == 0)
        {
            meshRenderer.enabled = false;
            cleaned = true;
            cobwebs.CleanWeb();
        }
    }
}