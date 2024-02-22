using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Experimental.Rendering.HDPipeline;

public class MirrorUpdate : MonoBehaviour
{
    ReflectionProbe probe;

    int frameTimer;

    void Start()
    {
        probe = GetComponent<ReflectionProbe>();
        probe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
        probe.timeSlicingMode = UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.NoTimeSlicing;
        probe.hdr = true;

        frameTimer = 0;
    }

    void Update()
    {
        frameTimer++;
        if (frameTimer >= 6)
        {
            frameTimer = 0;
            probe.RenderProbe();
            //HDAdditionalReflectionDataExtensions.RequestRenderNextUpdate(probe);
        }
    }
}