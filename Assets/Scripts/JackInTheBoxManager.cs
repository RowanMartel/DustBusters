using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackInTheBoxManager : Toy
{

    public GameObject go_lid;
    public GameObject go_clown;
    public GameObject go_handle;
    public AudioSource as_jackAudio;

    public AudioClip ac_jackMusic01;
    public AudioClip ac_jackMusic02;
    public AudioClip ac_jackLaugh;

    protected bool bl_turnHandle = false;
    protected bool bl_readyToPop = false;
    protected bool bl_sprung = false;

    private void Update()
    {
        if (bl_turnHandle)
        {
            //Rotates handle if handle needs rotating
            go_handle.transform.Rotate(0f, 0f, -1f, Space.Self);
            //Plays the next audio clip when the current audio clip is done
            if (!as_jackAudio.isPlaying)
            {
                if(as_jackAudio.clip == ac_jackMusic01 && GameManager.playerController.Go_heldObject != gameObject)
                {
                    GameManager.soundManager.PlayClip(ac_jackMusic02, as_jackAudio, false);
                }

                else if (as_jackAudio.clip == ac_jackMusic02)
                {
                    GameManager.soundManager.PlayClip(ac_jackMusic01, as_jackAudio, false);
                }
            }
        }
    }

    //Springs the Jack in the Box when picked up
    public override void Interact()
    {
        base.Interact();
        if (bl_readyToPop && !bl_sprung)
        {
            SpringJack();
        }
    }

    //Prepare jack in the box to spring
    public void ActivateJack()
    {
        bl_turnHandle = true;
        bl_readyToPop = true;
        GameManager.soundManager.PlayClip(ac_jackMusic01, as_jackAudio, false);
    }

    //Plays spring audio and animation
    public void SpringJack()
    {
        as_jackAudio.Stop();
        bl_readyToPop = false;
        bl_turnHandle = false;
        bl_sprung = true;
        GameManager.soundManager.PlayClip(ac_jackLaugh, as_jackAudio, true);
        LeanTween.rotateLocal(go_lid, new Vector3(0, 0, 0), 0.1f);
        LeanTween.scaleY(go_clown, 1f, 0.75f).setEaseOutElastic();
    }

}
