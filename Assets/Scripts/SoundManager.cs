using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    List<AudioClip> l_ac_curSFX = new();

    private IEnumerator coroutine;

    public void PlayClip(AudioClip ac_clip, AudioSource as_source)
    {
        if (l_ac_curSFX.Contains(ac_clip)) return;

        Vector3 v3_sourcePos = as_source.transform.position;
        if (Vector3.Distance(GameManager.playerController.transform.position, v3_sourcePos) >= Settings.int_SFXCullingDist)
            return;
        
        l_ac_curSFX.Add(ac_clip);

        as_source.PlayOneShot(ac_clip, Settings.flt_volume);

        coroutine = RemoveClip(ac_clip.length, ac_clip);
        StartCoroutine(coroutine);
    }

    IEnumerator RemoveClip(float flt_waitTime, AudioClip cl)
    {
        yield return new WaitForSeconds(flt_waitTime);
        l_ac_curSFX.Remove(cl);
    }
}