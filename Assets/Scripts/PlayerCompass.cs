using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCompass : MonoBehaviour
{
    // used by the tutorial to point towards the player's current target

    GameObject go_currentTarget;

    void Update()
    {
        transform.LookAt(go_currentTarget.transform.position);
        transform.Rotate(new Vector3(0, 180, 0));
    }

    public void SetTarget(GameObject go_newTarget)
    {
        go_currentTarget = go_newTarget;
    }
}