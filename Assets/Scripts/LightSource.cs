using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSource : MonoBehaviour
{
    //Add light to ghost's light source list
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ghost"))
        {
            GameManager.ghost.EnterLight(gameObject);
        }
    }

    //Remove light from ghost's light source list
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ghost"))
        {
            GameManager.ghost.ExitLight(gameObject);
        }
    }

}
