using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackInTheBoxManager : Pickupable
{

    public GameObject go_lid;
    public GameObject go_clown;
    public GameObject go_handle;
    public AudioSource as_jackAudio;

    public AudioClip jackMusic01;
    public AudioClip jackMusic02;
    public AudioClip jackLaugh;

    protected bool turnHandle = false;
    protected bool readyToPop = false;

    private void Update()
    {
        if (turnHandle)
        {
            go_handle.transform.Rotate(0f, 0f, -1f, Space.Self);
            if (!as_jackAudio.isPlaying && as_jackAudio.clip == jackMusic01)
            {
                turnHandle = false;
                readyToPop = true;
            }
        }

        if (GameManager.playerController.Go_heldObject == gameObject && readyToPop)
        {
            as_jackAudio.PlayOneShot(jackMusic02);
            as_jackAudio.PlayOneShot(jackLaugh);
            LeanTween.rotateLocal(go_lid, new Vector3(0, 0, 0), 0.1f);
            LeanTween.scaleY(go_clown, 1f, 0.75f).setEaseOutElastic();
            readyToPop = false;
        }
    }

    public override void Interact()
    {

    }

    public void ActivateJack()
    {
        turnHandle = true;
        as_jackAudio.clip = jackMusic01;
        as_jackAudio.Play();
    }


}
