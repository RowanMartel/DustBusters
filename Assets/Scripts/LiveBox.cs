using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveBox : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<Pickupable>() != null)
        {
            other.GetComponent<Pickupable>().Respawn();
        }
    }
}
