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
    [Tooltip("How many triggers is the player in?")]
    public int int_playerTriggerCount = 0;
    [Tooltip("Debug.Log?")]
    public bool bl_debug;
    [Tooltip("What regions can the player see the light from?")]
    public GameObject[] a_go_regions;

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

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Enter " + gameObject.name + "|" + other.gameObject.name);
        if(other.gameObject == GameManager.playerController.gameObject)
        {
            int_playerTriggerCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Exit " + gameObject.name + "|" + other.gameObject.name);
        if (other.gameObject == GameManager.playerController.gameObject)
        {
            int_playerTriggerCount--;
        }
    }

    // rotates the lightswitch model 180 degrees and then toggles the lights
    void Toggle()
    {
        //Causes problems for collisions. Swap mesh instead

        //if (bl_rotated)
        //    transform.Rotate(transform.right, 180, Space.Self);
        //else
        //    transform.Rotate(transform.forward, 180, Space.Self);

        bl_on = !bl_on;

        if (bl_on)
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

    private void Update()
    {
        if (bl_debug)
        {
            Debug.Log(int_playerTriggerCount);
        }
    }

}