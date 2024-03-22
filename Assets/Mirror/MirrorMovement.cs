using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorMovement : MonoBehaviour
{
    public Transform tfPlayerTarget;
    public Transform tfMirror;

    public int intXMod, intYMod, intZMod;

    public GameObject[] a_go_regions;

    void Update()
    {
        Vector3 v3LocalPlayer = tfMirror.InverseTransformPoint(tfPlayerTarget.position);
        //transform.position = tfMirror.TransformPoint(new Vector3(v3LocalPlayer.x, -v3LocalPlayer.y, v3LocalPlayer.z));

        Vector3 v3LookAtMirror = tfMirror.TransformPoint(new Vector3(intXMod * v3LocalPlayer.x, intYMod * v3LocalPlayer.y, intZMod * v3LocalPlayer.z));
        transform.LookAt(v3LookAtMirror);

        //GetComponent<Camera>().nearClipPlane = Vector3.Distance(transform.position, tfMirror.position) + 1;
    }

    public void ActivateCam()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateCam()
    {
        gameObject.SetActive(false);
    }

}