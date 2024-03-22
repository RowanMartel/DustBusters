using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVStatic : Interactable
{

    public AudioSource as_staticAudio;

    public GameObject go_staticScreen;

    public Shader sh_shader;

    public bool bl_powered;
    public bool bl_on;

    private void Start()
    {
        bl_powered = true;
    }

    //Checks if the tv is set to on and has power, and sets the static accordingly
    public void Refresh()
    {
        if(bl_powered == false || bl_on == false)
        {
            as_staticAudio.Pause();
            go_staticScreen.SetActive(false);
            return;
        }

        if (bl_on)
        {
            as_staticAudio.Play();
            go_staticScreen.SetActive(true);
        }

    }

    //Sets the tv to on then refreshes
    public void Activate()
    {
        bl_on = true;
        Refresh();
    }

    //Sets the tv to off then refreshes
    public void Deactivate()
    {
        bl_on = false;
        Refresh();
    }

    public override void Interact()
    {
        PlayerController pc_player = GameManager.playerController;
        if (pc_player.Go_heldObject == null) return;
        if (pc_player.Go_heldObject.GetComponent<Pickupable>().bl_remote && !bl_on) Activate();
        else if (pc_player.Go_heldObject.GetComponent<Pickupable>().bl_remote && bl_on) Deactivate();
    }
}
