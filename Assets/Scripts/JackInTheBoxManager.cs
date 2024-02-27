using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackInTheBoxManager : Pickupable
{

    public GameObject go_lid;
    public GameObject go_clown;
    public GameObject go_handle;

    protected int int_bounceCount = 0;
    protected float flt_countDown = 0;
    protected bool turnHandle = true;

    private void Update()
    {
        if (flt_countDown > 0)
        {
            flt_countDown -= Time.deltaTime;
        }
        else if(flt_countDown < 0)
        {
            LeanTween.rotateLocal(go_lid, new Vector3(0, 0, 0), 0.1f);
            LeanTween.scaleY(go_clown, 1f, 0.75f).setEaseOutElastic();
            flt_countDown = 0;
        }

        if(turnHandle)
        {
            go_handle.transform.Rotate(transform.forward);
        }
    }

    public override void Interact()
    {
        flt_countDown = 3;
        //OpenLid();
        //BouncyClown();
    }

    public void StartHandleSpin()
    {
        turnHandle = true;
    }


}
