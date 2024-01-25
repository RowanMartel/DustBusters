using UnityEngine;

public class Exit : Interactable
{
    [Tooltip("Put the unlock SFX here")]
    public AudioClip ac_unlock;
    AudioSource as_source;

    private void Start()
    {
        as_source = GetComponent<AudioSource>();
    }

    // ends the game if the player is holding the key and has the escape house task
    public override void Interact()
    {
        if (GameManager.playerController.Go_heldObject == null ||
            !GameManager.playerController.Go_heldObject.GetComponent<Pickupable>().bl_frontDoorKey ||
            !GameManager.taskManager.li_taskList.Contains(TaskManager.Task.EscapeHouse))
            return;

        GameManager.soundManager.PlayClip(ac_unlock, as_source);

        FindObjectOfType<MenuManager>().ToEnd();
    }
}