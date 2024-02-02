using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [HideInInspector] public bool bl_pickupable;

    public abstract void Interact();

    private void Awake()
    {
        GetComponent<Outline>().enabled = false;
    }
}