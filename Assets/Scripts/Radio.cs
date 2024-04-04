using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Radio : Interactable
{

    public AudioSource as_source;
    public AudioClip[] a_ac_broadcasts;
    public bool bl_playing = true;
    int int_curBroadcast = 0;
    public Material mat_on;
    public Material mat_off;
    bool bl_powered;

    public AudioMixer radioMixer;

    // Start is called before the first frame update
    void Start()
    {
        as_source.clip = a_ac_broadcasts[int_curBroadcast];
        as_source.Play();

        bl_powered = true;

        GameManager.menuManager.GamePaused += FadeOut;
        GameManager.menuManager.GameUnpaused += FadeIn;
        GameManager.menuManager.MusicVolumeChanged += UpdateVolume;

        as_source.volume = Settings.flt_musicVolume;
        radioMixer = as_source.outputAudioMixerGroup.audioMixer;
        radioMixer.SetFloat("RadioVolume", 0f);

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
        if(!as_source.isPlaying && !GameManager.menuManager.Bl_paused)
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
        if (bl_playing && bl_powered)
        {
            as_source.UnPause();
            //as_source.volume = 0;
            radioMixer.SetFloat("RadioVolume", -15f);
            LeanTween.value(-15f, 0, 1f).setOnUpdate(FadingUpdate).setIgnoreTimeScale(true);
        }
    }

    void FadeOut(object source, EventArgs e)
    {
        float value;
        radioMixer.GetFloat("RadioVolume", out value);
        if (bl_playing && bl_powered)
        {
            //as_source.volume = Settings.flt_musicVolume;
            LeanTween.value(value, -15f, 1f).setOnComplete(as_source.Pause).setOnUpdate(FadingUpdate).setIgnoreTimeScale(true);
        }
    }

    void FadingUpdate(float flt)
    {
        // as_source.volume = flt;
        radioMixer.SetFloat("RadioVolume", flt);
    }

    public void PowerOff()
    {
        as_source.volume = 0;
        bl_powered = false;
        GetComponent<Renderer>().material = mat_off;
    }

    public void PowerOn()
    {
        if (bl_playing)
        {
            as_source.volume = Settings.flt_musicVolume;
            GetComponent<Renderer>().material = mat_on;
        }
        bl_powered = true;
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
            if (bl_powered)
            {
                as_source.volume = Settings.flt_musicVolume;
                GetComponent<Renderer>().material = mat_on;
            }
        }
        else
        {
            bl_playing = false;
            as_source.volume = 0;
            GetComponent<Renderer>().material = mat_off;
        }
    }
}