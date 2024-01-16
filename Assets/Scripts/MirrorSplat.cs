using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MirrorSplat : MonoBehaviour
{
    [HideInInspector]
    public bool cleaned;
    int dirtLevel = 5;

    Renderer renderer;
    Mirror mirror;

    [Tooltip("Put the mopping SFX here")]
    public AudioClip cleanSFX;
    AudioSource audioSource;

    private void Start()
    {
        Physics.queriesHitTriggers = true;
        mirror = transform.GetComponentInParent<Mirror>();
        renderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnMouseExit()
    {
        if (cleaned || !mirror.gameActive) return;
        dirtLevel--;
        renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.g, renderer.material.color.a - .2f);
        audioSource.PlayOneShot(cleanSFX);
        if (dirtLevel == 0)
        {
            cleaned = true;
            mirror.CleanSplat();
        }
    }
}