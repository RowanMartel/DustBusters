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

    // ends the game if the player is holding the key and has the escape house task
    public override void Interact()
    {
        if (GameManager.playerController.go_heldObject == null ||
            !GameManager.playerController.go_heldObject.GetComponent<Pickupable>().frontDoorKey ||
            !GameManager.taskManager.taskList.Contains(TaskManager.Task.EscapeHouse))
            return;

        audioSource.PlayOneShot(unlockSFX);

        FindObjectOfType<RWMenu>().ToEnd();
    }
}