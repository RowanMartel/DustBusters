using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip ac_title;
    public AudioClip ac_pause;
    public AudioClip ac_dead;
    public AudioClip ac_credits;
    public AudioSource as_source;

    // assigns all event listeners, then plays the menu music
    private void Start()
    {
        as_source.volume = Settings.flt_musicVolume;

        GameManager.menuManager.GamePaused += PlayPause;
        GameManager.menuManager.GameUnpaused += StopPause;
        GameManager.menuManager.CreditsEntered += Credits;
        GameManager.menuManager.DeathScreenEntered += PlayDeathSound;
        GameManager.menuManager.StartTransitionToGame += GamePlay;
        GameManager.menuManager.StartQuitingGame += StopPause;
        GameManager.menuManager.MenuEntered += Title;
        GameManager.menuManager.MusicVolumeChanged += UpdateVolume;

        Title(this, new EventArgs());
    }

    // plays pause music when it receives the game paused event
    void PlayPause(object source, EventArgs e)
    {
        as_source.clip = ac_pause;
        FadeIn();
    }

    // stops pause music when it receives the game unpaused event
    void StopPause(object source, EventArgs e)
    {
        FadeOut();
    }

    // plays death sound when it receives the player death event
    void PlayDeathSound(object source, EventArgs e)
    {
        Debug.Log("Death Sound Triggered");
        Debug.Log(as_source.clip.name);
        Debug.Log(as_source.volume);

        as_source.Stop();
        as_source.PlayOneShot(ac_dead);
    }

    // stops all music when it receives the credits entered event
    void Credits(object source, EventArgs e)
    {
        as_source.clip = ac_credits;
        FadeIn();
    }

    // plays title them when it receives the menu entered event
    void Title(object source, EventArgs e)
    {
        as_source.Stop();
        as_source.clip = ac_title;
        FadeIn();
    }

    // stops all music when it receives the gameplay entered event
    void GamePlay(object source, EventArgs e)
    {
        FadeOut();
    }

    void FadeIn()
    {
        as_source.volume = 0;
        LeanTween.value(0f, Settings.flt_musicVolume, 1.5f).setOnUpdate(FadingUpdate).setIgnoreTimeScale(true);
        as_source.Play();
    }

    void FadeOut()
    {
        as_source.volume = Settings.flt_musicVolume;
        LeanTween.value(as_source.volume, 0, 1.5f).setOnComplete(AsStop).setOnUpdate(FadingUpdate).setIgnoreTimeScale(true);
    }

    void AsStop()
    {
        as_source.Stop();
        as_source.volume = Settings.flt_musicVolume;
    }

    void FadingUpdate(float flt)
    {
        as_source.volume = flt;
    }

    // updates the music volume when the menu slider is changed
    void UpdateVolume(object source, EventArgs e)
    {
        as_source.volume = Settings.flt_musicVolume;
    }
}