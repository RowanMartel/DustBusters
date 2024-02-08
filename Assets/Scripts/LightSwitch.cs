using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : Interactable
{
    [Tooltip("Modify in inspector to determine starting state")]
    public bool bl_on;
    [Tooltip("Add all lights this controls")]
    public List<GameObject> li_go_lights;
    [Tooltip("The light collider this controls")]
    public Collider lightCollider;
    [Tooltip("Toggle if the model itself is rotated 90 degrees")]
    public bool bl_rotated;
    [Tooltip("What regions can the player see the light from?")]
    public GameObject[] a_go_regions;
    [Tooltip("Depicts whether the fusebox is supplying power to the switch")]
    public bool bl_fuseActive;

    // toggle all lights on or off at start
    private void Start()
    {
        if (bl_on)
        {
            foreach (GameObject go_light in li_go_lights)
                go_light.SetActive(true);
            lightCollider.enabled = true;
        }
        else
        {
            foreach (GameObject go_light in li_go_lights)
                go_light.SetActive(false);
            lightCollider.enabled = false;
        }
    }

    public override void Interact()
    {
        Toggle();
    }

    public void SetFuseActive(bool bl_turningOn)
    {
        bl_fuseActive = bl_turningOn;

        if (bl_fuseActive && bl_on)
        {
            foreach (GameObject light in li_go_lights)
                light.SetActive(true);
            lightCollider.enabled = true;

            return;
        }

        foreach (GameObject light in li_go_lights)
            light.SetActive(false);
        lightCollider.enabled = false;
    }

    // rotates the lightswitch model 180 degrees and then toggles the lights
    void Toggle()
    {
        if (bl_rotated)
            transform.Rotate(transform.right, 180, Space.Self);
        else
            transform.Rotate(transform.forward, 180, Space.Self);

        bl_on = !bl_on;

        AudioSource as_source = GetComponent<AudioSource>();
        GameManager.soundManager.PlayClip(as_source.clip, as_source);

        if (bl_on && bl_fuseActive)
        {
            foreach (GameObject light in li_go_lights)
                light.SetActive(true);
            lightCollider.enabled = true;
        }
        else
        {
            foreach (GameObject light in li_go_lights)
                light.SetActive(false);
            lightCollider.enabled = false;
        }
    }
}