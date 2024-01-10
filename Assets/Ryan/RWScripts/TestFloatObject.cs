using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFloatObject : MonoBehaviour
{
    public bool canFloat;
    public bool isFloating;
    GhostBehavior ghost;

    private void Start()
    {
        canFloat = true;
        ghost = FindAnyObjectByType<GhostBehavior>();
    }

}
