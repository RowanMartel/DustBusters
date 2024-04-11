using System.Collections.Generic;
using UnityEngine;

public class Candle: Pickupable
{
    [Tooltip("Put the fire particle effect / light object here")]
    public GameObject go_fireFX;
    public ParticleSystem ps_fire;
    public AudioClip ac_candleLightSound;
    public AudioSource as_candleSoundSource;
    public bool bl_lit;

    protected override void Start()
    {
        base.Start();

        ps_fire.Stop();

        bl_lit = false;
    }

    // light the candle if the player is holding the right item
    public override void Interact()
    {
        if (GameManager.playerController.Go_heldObject == null || !GameManager.playerController.Go_heldObject.GetComponent<Pickupable>().bl_lighter || bl_lit == true) return;
        Light();
    }

    // start the vfx and sfx
    public void Light()
    {
        GameManager.soundManager.PlayClip(ac_candleLightSound, as_candleSoundSource, true);

        //Activate light and audio
        go_fireFX.SetActive(true);

        //Activate particles
        ps_fire.Play();
        ps_fire.Clear();

        bl_lit = true;
    }

    // stop the vfx and sfx
    public void UnLight()
    {
        //Stop light and audio
        go_fireFX.SetActive(false);


        //Stop particles
        ps_fire.Stop();
        ps_fire.Clear();

        bl_lit = false;
    }

}