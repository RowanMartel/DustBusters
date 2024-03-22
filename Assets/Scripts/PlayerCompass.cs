using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCompass : MonoBehaviour
{
    GameObject go_currentTarget;

    void Update()
    {
        transform.LookAt(go_currentTarget.transform.position);
    }

    public void SetTarget(GameObject go_newTarget)
    {
        go_currentTarget = go_newTarget;
    }
}