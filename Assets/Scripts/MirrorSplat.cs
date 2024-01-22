using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MirrorSplat : MonoBehaviour
{
    [HideInInspector]
    public bool bl_cleaned;
    int int_dirtLevel = 5;

    Renderer ren;
    Mirror mirror;

    [Tooltip("Put the mopping SFX here")]
    public AudioClip ac_clean;
    AudioSource as_clean;

    private void Start()
    {
        Physics.queriesHitTriggers = true;
        mirror = transform.GetComponentInParent<Mirror>();
        ren = GetComponent<Renderer>();
        as_clean = GetComponent<AudioSource>();
    }

    private void OnMouseExit()
    {
        if (bl_cleaned || !mirror.bl_gameActive) return;
        int_dirtLevel--;
        ren.material.color = new Color(ren.material.color.r, ren.material.color.g, ren.material.color.g, ren.material.color.a - .2f);
        as_clean.PlayOneShot(ac_clean);
        if (int_dirtLevel == 0)
        {
            bl_cleaned = true;
            mirror.CleanSplat();
        }
    }
}