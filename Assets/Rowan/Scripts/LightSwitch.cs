using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Interactable
{
    [Tooltip("Modify in inspector to determine starting state")]
    public bool on;

    [Tooltip("Add all lights this controls")]
    public List<GameObject> lights;

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
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 180);
        on = !on;

        if (on)
            foreach (GameObject light in lights)
                light.SetActive(true);
        else
            foreach (GameObject light in lights)
                light.SetActive(false);
    }
}