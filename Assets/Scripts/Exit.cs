using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : Interactable
{
    [Tooltip("Put the unlock SFX here")]
    public AudioClip unlockSFX;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void Interact()
    {
        if (GameManager.playerController.heldObject == null ||
            !GameManager.playerController.heldObject.GetComponent<Pickupable>().frontDoorKey ||
            !GameManager.taskManager.taskList.Contains(TaskManager.Task.EscapeHouse))
            return;

        audioSource.PlayOneShot(unlockSFX);

        // start game end sequence();
    }
}