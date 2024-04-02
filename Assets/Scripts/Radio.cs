using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : Interactable
{

    public AudioSource as_source;
    public AudioClip[] a_ac_broadcasts;
    bool bl_playing = true;
    int int_curBroadcast = 0;
    public Material mat_on;
    public Material mat_off;

    // Start is called before the first frame update
    void Start()
    {
        as_source.clip = a_ac_broadcasts[int_curBroadcast];
        as_source.Play();

        GameManager.menuManager.GamePaused += FadeOut;
        GameManager.menuManager.GameUnpaused += FadeIn;
        GameManager.menuManager.MusicVolumeChanged += UpdateVolume;

        as_source.volume = Settings.flt_musicVolume;
    }

    private void OnDestroy()
    {
        GameManager.menuManager.GamePaused -= FadeOut;
        GameManager.menuManager.GameUnpaused -= FadeIn;
        GameManager.menuManager.MusicVolumeChanged -= UpdateVolume;
    }
    // Update is called once per frame
    void Update()
    {
        if(bl_playing && !as_source.isPlaying)
        {
            int_curBroadcast++;
            if(int_curBroadcast >=  a_ac_broadcasts.Length)
            {
                int_curBroadcast = 0;
            }
            as_source.clip = a_ac_broadcasts[int_curBroadcast];
            as_source.Play();
        }
    }

    void FadeIn(object source, EventArgs e)
    {
        if (bl_playing)
        {
            as_source.volume = 0;
            LeanTween.value(0f, Settings.flt_musicVolume, 2f).setOnUpdate(FadingUpdate).setIgnoreTimeScale(true);
            as_source.Play();
        }
    }

    void FadeOut(object source, EventArgs e)
    {
        if (bl_playing)
        {
            as_source.volume = Settings.flt_volume;
            LeanTween.value(as_source.volume, 0, 2f).setOnComplete(as_source.Stop).setOnUpdate(FadingUpdate).setIgnoreTimeScale(true);
        }
    }

    void FadingUpdate(float flt)
    {
        as_source.volume = flt;
    }

    // updates the music volume when the menu slider is changed
    void UpdateVolume(object source, EventArgs e)
    {
        if(as_source != null)
            as_source.volume = Settings.flt_musicVolume;
    }

    public override void Interact()
    {
        if (!bl_playing)
        {
            bl_playing = true;
            GetComponent<Renderer>().material = mat_on;
        }
        else
        {
            bl_playing = false;
            as_source.Stop();
            GetComponent<Renderer>().material = mat_off;
        }
    }

}