using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    List<AudioClip> l_ac_curSFX = new();

    public void PlayClip(AudioClip ac_clip, AudioSource as_source)
    {
        if (l_ac_curSFX.Contains(ac_clip)) return;

        Vector3 v3_sourcePos = as_source.transform.position;
        if (Vector3.Distance(GameManager.playerController.transform.position, v3_sourcePos) >= Settings.int_SFXCullingDist)
            return;

        as_source.PlayOneShot(ac_clip, Settings.flt_volume);
    }
}