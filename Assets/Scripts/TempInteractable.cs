using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempInteractable : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Outline outline = transform.GetComponent<Outline>();
        outline.enabled = false;
        outline.OutlineWidth = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
