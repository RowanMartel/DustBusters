using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckZone : MonoBehaviour
{
    // used by the tutorial to check when the player is in a certain position

    public bool bl_playerIn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            bl_playerIn = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            bl_playerIn = false;
    }
}