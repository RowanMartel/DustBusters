using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : MonoBehaviour
{
    public int int_framesToRend;
    int cur_framesToRend;
    public Camera[] a_cams;


    // Update is called once per frame
    void Update()
    {
        cur_framesToRend++;
        if (cur_framesToRend >= int_framesToRend)
        {
            cur_framesToRend = 0;
            foreach (Camera cam in a_cams)
            {
                cam.Render();
            }
        }
    }
}
