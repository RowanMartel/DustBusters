using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FloorSplat : MonoBehaviour
{
    [HideInInspector]
    public bool bl_cleaned;
    int int_dirtLevel = 5;

    Renderer ren;
    FloorMess floorMess;

    [Tooltip("Put the mopping SFX here")]
    public AudioClip ac_clean;
    AudioSource as_clean;

    private void Start()
    {
        Physics.queriesHitTriggers = true;
        floorMess = transform.GetComponentInParent<FloorMess>();
        ren = GetComponent<Renderer>();
        as_clean = GetComponent<AudioSource>();
    }

    private void OnMouseExit()
    {
        if (bl_cleaned || !floorMess.bl_gameActive) return;
        int_dirtLevel--;
        ren.material.color = new Color(ren.material.color.r, ren.material.color.g, ren.material.color.g, ren.material.color.a - .2f);
        as_clean.PlayOneShot(ac_clean);
        if (int_dirtLevel == 0)
        {
            bl_cleaned = true;
            floorMess.CleanSplat();
        }
    }
}