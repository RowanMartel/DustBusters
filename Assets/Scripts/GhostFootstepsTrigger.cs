using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFootstepsTrigger : MonoBehaviour
{
    AudioSource as_source;
    [SerializeField] AudioClip ac_footsteps;

    void Start()
    {
        // child of ghost
        as_source = GetComponentInParent<AudioSource>();
    }


    // when player enters trigger, play footsteps sound if not already playing anything
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || !as_source.isPlaying)
            GameManager.soundManager.PlayClip(ac_footsteps, as_source, true);
    }
}