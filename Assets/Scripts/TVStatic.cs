using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVStatic : MonoBehaviour
{

    public AudioSource as_staticAudio;

    public GameObject go_staticScreen;

    public Shader sh_shader;

    public List<float> l_flt_chancePerAggro;

    /*private void Start()
    {
        
    }*/

    public void Activate()
    {
        as_staticAudio.Play();
        go_staticScreen.SetActive(true);
    }

    public void Deactivate()
    {
        as_staticAudio.Stop();
        go_staticScreen.SetActive(false);
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
