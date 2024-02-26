using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackInTheBoxManager : Pickupable
{

    public GameObject go_lid;
    public GameObject go_clown;

    protected int int_bounceCount = 0;

    void OpenLid()
    {
        LeanTween.rotateLocal(go_lid, new Vector3(0, 0, 0), 0.1f);
    }

    void BouncyClown()
    {
        switch(int_bounceCount)
        {
            case 0:
                int_bounceCount++;
                LeanTween.scaleY(go_clown, 1.15f, 0.1f).setOnComplete(BouncyClown);
                break;

            case 1:
                int_bounceCount++;
                LeanTween.scaleY(go_clown, 0.9f, 0.13f).setOnComplete(BouncyClown);
                break;

            case 2:
                int_bounceCount++;
                LeanTween.scaleY(go_clown, 1.05f, 0.17f).setOnComplete(BouncyClown);
                break;

            case 3:
                LeanTween.scaleY(go_clown, 1f, 0.22f);
                break;

        }
    }
    public override void Interact()
    {
        OpenLid();
        BouncyClown();
    }


}
