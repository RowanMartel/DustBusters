using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICompass : MonoBehaviour
{
    [SerializeField] GameObject go_target;

    void Update()
    {
        transform.right = Camera.main.WorldToScreenPoint(go_target.transform.position) - transform.position;
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - 90);
    }
}