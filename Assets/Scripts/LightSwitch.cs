using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Interactable
{
    [Tooltip("Modify in inspector to determine starting state")]
    public bool on;

    [Tooltip("Add all lights this controls")]
    public List<GameObject> lights;
    [Tooltip("The light collider this controls")]
    public Collider lightCollider;

    public bool rotated;

    private void Start()
    {
        if (on)
            foreach (GameObject light in lights)
                light.SetActive(true);
        else
            foreach (GameObject light in lights)
                light.SetActive(false);
    }

    public override void Interact()
    {
        Toggle();
    }

    void Toggle()
    {
        if (rotated)
            transform.Rotate(transform.right, 180, Space.Self);
        else
            transform.Rotate(transform.forward, 180, Space.Self);

        on = !on;

        if (on)
        {
            foreach (GameObject light in lights)
                light.SetActive(true);
            lightCollider.enabled = true;
        }
        else
        {
            foreach (GameObject light in lights)
                light.SetActive(false);
            lightCollider.enabled = false;
        }
    }
}