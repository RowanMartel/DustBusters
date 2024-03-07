using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVStaticTrigger : MonoBehaviour
{
    public List<float> l_flt_chancePerAggro;
    protected TVStatic tvStatic;
    // Start is called before the first frame update
    void Awake()
    {
        tvStatic = FindObjectOfType<TVStatic>();
    }

    //Has a chance to Activate when ghost enters collider
    private void OnTriggerEnter(Collider other)
    {
        GhostBehavior gb_ghost = other.gameObject.GetComponent<GhostBehavior>();
        if (gb_ghost != null)
        {
            float flt_chance = Random.Range(0f, 100f);
            if (flt_chance <= l_flt_chancePerAggro[gb_ghost.int_curAggressionLevel - 1])
            {
                tvStatic.Activate();
            }
        }
    }
}
