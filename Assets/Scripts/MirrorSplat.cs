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

    public Material mat_bloody;

    private void Start()
    {
        Physics.queriesHitTriggers = true;
        mirror = transform.GetComponentInParent<Mirror>();
        ren = GetComponent<Renderer>();
        as_clean = GetComponent<AudioSource>();
    }

    // cleans the splat a little whenever the mouse goes over it
    private void OnMouseExit()
    {
        if (bl_cleaned || !mirror.bl_gameActive || mirror.bl_paused) return;
        int_dirtLevel--;
        ren.material.color = new Color(ren.material.color.r, ren.material.color.g, ren.material.color.g, ren.material.color.a - .2f);
        GameManager.soundManager.PlayClip(ac_clean, as_clean, true);
        if (int_dirtLevel == 0)
        {
            bl_cleaned = true;
            mirror.CleanSplat();
        }
    }

    // makes the splat dirty again, possibly bloody
    public void ReDirty(bool bl_bloody = false)
    {
        //To implement when we have bloody material
        //if (bl_bloody) GetComponent<Renderer>().material = mat_bloody;

        bl_cleaned = false;
        ren.material.color = new Color(ren.material.color.r, ren.material.color.g, ren.material.color.g, 1);
        int_dirtLevel = 5;
    }
}