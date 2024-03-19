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
    public AudioSource as_source;

    // assigns all event listeners, then plays the menu music
    private void Start()
    {
        GameManager.menuManager.GamePaused += PlayPause;
        GameManager.menuManager.GameUnpaused += StopPause;
        GameManager.menuManager.CreditsEntered += Credits;
        GameManager.menuManager.DeathScreenEntered += PlayDeathSound;
        GameManager.menuManager.GameStart += GamePlay;
        GameManager.menuManager.MenuEntered += Title;

        Title(this, new EventArgs());
    }

    // plays pause music when it receives the game paused event
    void PlayPause(object source, EventArgs e)
    {
        as_source.clip = ac_pause;
        as_source.Play();
    }

    // stops pause music when it receives the game unpaused event
    void StopPause(object source, EventArgs e)
    {
        as_source.Stop();
    }

    // plays death sound when it receives the player death event
    void PlayDeathSound(object source, EventArgs e)
    {
        as_source.Stop();
        as_source.PlayOneShot(ac_dead);
    }

    // stops all music when it receives the credits entered event
    void Credits(object source, EventArgs e)
    {
        as_source.Stop();
    }

    // plays title them when it receives the menu entered event
    void Title(object source, EventArgs e)
    {
        as_source.Stop();
        as_source.clip = ac_title;
        as_source.Play();
    }

    // stops all music when it receives the gameplay entered event
    void GamePlay(object source, EventArgs e)
    {
        as_source.Stop();
    }
}