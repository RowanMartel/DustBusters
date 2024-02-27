using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackInTheBoxManager : Pickupable
{

    public GameObject go_lid;
    public GameObject go_clown;
    public GameObject go_handle;
    public AudioSource as_jackAudio;

    public AudioClip ac_jackMusic01;
    public AudioClip ac_jackMusic02;
    public AudioClip ac_jackLaugh;

    protected bool turnHandle = false;
    protected bool readyToPop = false;

    private void Update()
    {
        if (turnHandle)
        {
            go_handle.transform.Rotate(0f, 0f, -1f, Space.Self);
            if (!as_jackAudio.isPlaying)
            {
                if(as_jackAudio.clip == ac_jackMusic01 && GameManager.playerController.Go_heldObject != gameObject)
                {
                    GameManager.soundManager.PlayClip(ac_jackMusic02, as_jackAudio);
                }

                else if (as_jackAudio.clip == ac_jackMusic02)
                {
                    GameManager.soundManager.PlayClip(ac_jackMusic01, as_jackAudio);
                }

                else if (as_jackAudio.clip == ac_jackMusic01 && GameManager.playerController.Go_heldObject == gameObject)
                {
                    turnHandle = false;
                    GameManager.soundManager.PlayClip(ac_jackLaugh, as_jackAudio);
                    LeanTween.rotateLocal(go_lid, new Vector3(0, 0, 0), 0.1f);
                    LeanTween.scaleY(go_clown, 1f, 0.75f).setEaseOutElastic();
                }
            }
        }
    }

    public override void Interact()
    {
        ActivateJack();
    }

    public void ActivateJack()
    {
        turnHandle = true;
        GameManager.soundManager.PlayClip(ac_jackMusic01, as_jackAudio);
    }


}
