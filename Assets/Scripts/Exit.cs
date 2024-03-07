using UnityEngine;

public class Exit : Interactable
{
    [Tooltip("Put the unlock SFX here")]
    public AudioClip ac_unlock;
    AudioSource as_source;

    protected GameObject parent;

    private void Start()
    {
        parent = transform.parent.gameObject;
        as_source = GetComponent<AudioSource>();
    }

    // ends the game if the player is holding the key and has the escape house task
    public override void Interact()
    {
        if (GameManager.playerController.Go_heldObject == null ||
            !GameManager.playerController.Go_heldObject.GetComponent<Pickupable>().bl_frontDoorKey ||
            !GameManager.taskManager.li_taskList.Contains(TaskManager.Task.EscapeHouse))
            return;

        GameManager.soundManager.PlayClip(ac_unlock, as_source, true);

        GameManager.playerController.En_state = PlayerController.State.inactive;
        Time.timeScale = 0;
        LeanTween.rotateLocal(parent, new Vector3(parent.transform.rotation.x, parent.transform.rotation.y + 125, parent.transform.rotation.z), 3).setEase(LeanTweenType.easeOutSine).setOnComplete(GoToEnd).setIgnoreTimeScale(true);
    }

    private void GoToEnd()
    {
        FindObjectOfType<MenuManager>().ToEnd();
    }
}