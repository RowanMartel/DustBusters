using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaningWater : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Pickupable pickupable = other.GetComponent<Pickupable>();
        if (!pickupable || !pickupable.dirtyDish) return;

        pickupable.Clean();
    }
}