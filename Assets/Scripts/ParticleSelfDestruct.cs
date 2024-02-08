using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSelfDestruct : MonoBehaviour
{
    public float flt_duration;

    // Update is called once per frame
    void Update()
    {
        flt_duration -= Time.deltaTime;
        if(flt_duration <= 0)
        {
            Destroy(gameObject);
        }
    }
}
