using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVStatic : MonoBehaviour
{

    public AudioSource as_staticAudio;

    public GameObject go_staticScreen;

    public Shader sh_shader;

    public List<float> l_flt_chancePerAggro;

    public bool bl_powered;
    public bool bl_on;

    private void Start()
    {
        bl_powered = true;
    }

    //Checks if the tv is set to on and has power, and sets the static accordingly
    public void Refresh()
    {
        if(bl_powered == false || bl_on == false)
        {
            as_staticAudio.Pause();
            go_staticScreen.SetActive(false);
            return;
        }

        if (bl_on)
        {
            as_staticAudio.Play();
            go_staticScreen.SetActive(true);
        }

    }

    //Sets the tv to on then refreshes
    public void Activate()
    {
        bl_on = true;
        Refresh();
    }

    //Sets the tv to off then refreshes
    public void Deactivate()
    {
        bl_on = false;
        Refresh();
    }

    //Has a chance to Activate when ghost enters collider
    private void OnTriggerEnter(Collider other)
    {
        GhostBehavior gb_ghost = other.gameObject.GetComponent<GhostBehavior>();
        if(gb_ghost != null)
        {
            float flt_chance = Random.Range(0f, 100f);
            if(flt_chance <= l_flt_chancePerAggro[gb_ghost.int_curAggressionLevel - 1])
            {
                Activate();
            }
        }
    }


}
