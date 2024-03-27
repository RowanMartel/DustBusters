using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMovement : MonoBehaviour
{
    public Transform tfPlayerTarget;
    public Transform tfMirror;

    public int intXMod, intYMod, intZMod;

    public GameObject[] a_go_regions;

    bool bl_active;
    public int int_framesToRend;
    int cur_framesToRend;
    Camera cam;
    public GameObject go_renderObj;
    public Renderer ren_renderer;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (ren_renderer.isVisible)
        {
            go_renderObj.SetActive(true);
        }
        else
        {
            go_renderObj.SetActive(false);
        }

        if(bl_active)
        {
            cur_framesToRend++;
            if(cur_framesToRend >= int_framesToRend)
            {
                cur_framesToRend = 0;
                if (ren_renderer.isVisible)
                {
                    Vector3 v3LocalPlayer = tfMirror.InverseTransformPoint(tfPlayerTarget.position);
                    //transform.position = tfMirror.TransformPoint(new Vector3(v3LocalPlayer.x, -v3LocalPlayer.y, v3LocalPlayer.z));

                    Vector3 v3LookAtMirror = tfMirror.TransformPoint(new Vector3(intXMod * v3LocalPlayer.x, intYMod * v3LocalPlayer.y, intZMod * v3LocalPlayer.z));
                    transform.LookAt(v3LookAtMirror);

                    cam.Render();

                    //GetComponent<Camera>().nearClipPlane = Vector3.Distance(transform.position, tfMirror.position) + 1;
                }
            }
        }
    }

    public void ActivateCam()
    {
        bl_active = true;
        //gameObject.SetActive(true);
    }

    public void DeactivateCam()
    {
        bl_active = false;
        //gameObject.SetActive(false);
    }

}