using System;
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
    [Tooltip("Material for light fixture for when light is off")]
    public Material mat_off;
    [Tooltip("Material for light fixture for when light is on")]
    public Material mat_on;
    [Tooltip("LightFixtures Renderers")]
    public Renderer[] a_ren_fixturesGlass;
    public LightSwitch[] l_ls_secondarySwitches;
    public LightSwitch ls_primarySwitch;
    public bool bl_isSecondarySwitch;

    // event for when any lightswitch is toggled, used by tutorial
    public static event EventHandler<EventArgs> AnySwitchToggled;

    // toggle all lights on or off at start
    private void Start()
    {
        if (bl_on)
        {
            foreach (GameObject go_light in li_go_lights)
                go_light.SetActive(true);
            lightCollider.enabled = true;
            foreach (Renderer ren in a_ren_fixturesGlass)
            {
                ren.material = mat_on;
            }
        }
        else
        {
            foreach (GameObject go_light in li_go_lights)
                go_light.SetActive(false);
            lightCollider.enabled = false;
            foreach (Renderer ren in a_ren_fixturesGlass)
            {
                ren.material = mat_off;
            }
        }
    }

    public override void Interact()
    {
        Toggle();
    }

    //Lets the lightswitch know whether it should have power
    public void SetFuseActive(bool bl_turningOn)
    {
        bl_fuseActive = bl_turningOn;

        //Turns on light if the fuse and the switch are on.
        if (bl_fuseActive && bl_on)
        {
            foreach (GameObject light in li_go_lights)
                light.SetActive(true);
            lightCollider.enabled = true;
            foreach (Renderer ren in a_ren_fixturesGlass)
            {
                ren.material = mat_on;
            }
            return;
        }

        //Otherwise turn the light off
        foreach (GameObject light in li_go_lights)
            light.SetActive(false);
        foreach (Renderer ren in a_ren_fixturesGlass)
        {
            ren.material = mat_off;
        }
        lightCollider.enabled = false;
    }

    // rotates the lightswitch model 180 degrees and then toggles the lights
    void Toggle()
    {
        AnySwitchToggled?.Invoke(this, new EventArgs());

        AudioSource as_source = GetComponent<AudioSource>();
        GameManager.soundManager.PlayClip(as_source.clip, as_source, true);

        //If this isn't the primary lightswitch, then tell the primary lightswitch to Toggle
        if (bl_isSecondarySwitch)
        {
            ls_primarySwitch.Toggle();
            return;
        }

        //If this is the primary lightswitch
        //Tell all connected lightswitches to rotate and swap it's state
        foreach(LightSwitch ls in l_ls_secondarySwitches)
        {
            if (ls.bl_rotated)
                ls.transform.Rotate(transform.right, 180, Space.Self);
            else
                ls.transform.Rotate(transform.forward, 180, Space.Self);

            ls.bl_on = !ls.bl_on;
        }

        //Rotate self
        if (bl_rotated)
            transform.Rotate(transform.right, 180, Space.Self);
        else
            transform.Rotate(transform.forward, 180, Space.Self);

        //Swap state
        bl_on = !bl_on;

        if (bl_on && bl_fuseActive)
        {
            //Turns the lights on if both the fuse and the switch are on
            foreach (GameObject light in li_go_lights)
                light.SetActive(true);
            foreach(Renderer ren in a_ren_fixturesGlass)
            {
                ren.material = mat_on;
            }
            lightCollider.enabled = true;
        }
        else
        {
            //Otherwise turn the lights off
            foreach (GameObject light in li_go_lights)
                light.SetActive(false);
            foreach (Renderer ren in a_ren_fixturesGlass)
            {
                ren.material = mat_off;
            }
            lightCollider.enabled = false;
        }
    }
}