using UnityEngine;

public class Mirror : Interactable
{
    [Tooltip("Put the amount of splats the player needs to clean here")]
    public int splats;
    [HideInInspector] public bool gameActive = false;
    bool clean = false;

    Collider collider;

    private void Start()
    {
        collider = GetComponent<Collider>();
    }

    // toggles the mirror cleaning minigame if the player is holding the right object
    public override void Interact()
    {
        if (GameManager.playerController.Go_heldObject == null ||
            !GameManager.playerController.Go_heldObject.GetComponent<Pickupable>().duster)
            return;

        if (clean) return;

        gameActive = !gameActive;

        if (gameActive)
        {
            Cursor.lockState = CursorLockMode.Confined;
            GameManager.playerController.En_state = PlayerController.State.inactive;
            collider.enabled = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            GameManager.playerController.En_state = PlayerController.State.active;
            collider.enabled = true;
        }
    }

    // ticks down splats int, then checks if the minigame is complete
    public void CleanSplat()
    {
        splats--;
        if (splats <= 0)
        {
            GameManager.taskManager.CompleteTask(TaskManager.Task.CleanMirror);
            Cursor.lockState = CursorLockMode.Locked;
            GameManager.playerController.En_state = PlayerController.State.active;
            gameActive = false;
            clean = true;
        }
    }
}