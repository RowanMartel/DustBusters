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
        Activate();
    }

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

    public void Activate()
    {
        bl_on = true;
        Refresh();
    }

    public void Deactivate()
    {
        bl_on = false;
        Refresh();
    }

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
