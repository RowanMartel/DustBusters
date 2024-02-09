using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EasterEggPicture : MonoBehaviour
{
    public Image img_image;
    public Sprite spt_normal;
    public Sprite spt_after;

    //Swap image for easter egg image
    public void Switch()
    {
        if(img_image.sprite != spt_after)
        {
            img_image.sprite = spt_after;
        }
    }
}
