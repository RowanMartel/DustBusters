using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    List<AudioClip> l_ac_curSFX = new();

    private IEnumerator coroutine;

    public int int_attemptsToPickNewSound;

    // plays the given clip in the given source if it isn't already playing and if it's near the player
    public void PlayClip(AudioClip ac_clip, AudioSource as_source)
    {
        if (as_source.isPlaying) return;

        if (l_ac_curSFX.Contains(ac_clip)) return;

        Vector3 v3_sourcePos = as_source.transform.position;
        if (Vector3.Distance(GameManager.playerController.transform.position, v3_sourcePos) >= Settings.int_SFXCullingDist)
            return;
        
        l_ac_curSFX.Add(ac_clip);

        as_source.PlayOneShot(ac_clip, Settings.flt_volume);

        coroutine = RemoveClip(ac_clip.length, ac_clip);
        StartCoroutine(coroutine);
    }

    //Picks a random clip from a list of clips, then plays that clip with the other PlayClip method
    public void PlayClip(AudioClip[] a_ac_clips, AudioSource as_source)
    {
        if (as_source.isPlaying) return;

        AudioClip ac_clip;
        int attempts = 0;
        do
        {
            ac_clip = a_ac_clips[Random.Range(0, a_ac_clips.Length)];
            attempts++;
        }while(l_ac_curSFX.Contains(ac_clip) && attempts >= int_attemptsToPickNewSound);

        PlayClip(ac_clip, as_source);
    }

    // removes the clip from the currently playing list after the duration of the clip has passed
    IEnumerator RemoveClip(float flt_waitTime, AudioClip cl)
    {
        yield return new WaitForSeconds(flt_waitTime);
        l_ac_curSFX.Remove(cl);
    }
}