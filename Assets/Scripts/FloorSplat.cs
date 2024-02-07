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

    public Material mat_bloody;

    public GameObject go_dustParticles;
    public Color clr_dustColor;

    private void Start()
    {
        Physics.queriesHitTriggers = true;
        floorMess = transform.GetComponentInParent<FloorMess>();
        ren = GetComponent<Renderer>();
        as_clean = GetComponent<AudioSource>();
    }

    // cleans the splat a little whenever the mouse goes over it
    private void OnMouseExit()
    {
        if (bl_cleaned || !floorMess.bl_gameActive) return;
        int_dirtLevel--;
        ren.material.color = new Color(ren.material.color.r, ren.material.color.g, ren.material.color.g, ren.material.color.a - .2f);
        GameManager.soundManager.PlayClip(ac_clean, as_clean);
        GameObject go_dust = Instantiate(go_dustParticles, transform);
        go_dust.GetComponent<ParticleSystem>().startColor = clr_dustColor;
        if (int_dirtLevel == 0)
        {
            bl_cleaned = true;
            floorMess.CleanSplat();
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