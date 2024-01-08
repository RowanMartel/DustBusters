using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupboardTrigger : MonoBehaviour
{
    public List<Pickupable> dishes;

    private void OnTriggerEnter(Collider other)
    {
        Pickupable pickupable = other.GetComponent<Pickupable>();
        if (!pickupable || !pickupable.dish || pickupable.dirtyDish) return;

        pickupable.inCupboard = true;
    }
    private void OnTriggerExit(Collider other)
    {
        Pickupable pickupable = other.GetComponent<Pickupable>();
        if (!pickupable || !pickupable.dish || pickupable.dirtyDish) return;

        pickupable.inCupboard = false;
    }
}