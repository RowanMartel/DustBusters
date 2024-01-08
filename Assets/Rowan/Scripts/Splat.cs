using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Splat : MonoBehaviour, IPointerExitHandler
{
    [HideInInspector]
    public bool cleaned;
    int dirtLevel = 5;

    Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cleaned) return;
        else
        {
            dirtLevel--;
            image.color = new Color(image.color.r, image.color.g, image.color.g, image.color.a - .2f);
            if (dirtLevel == 0) cleaned = true;
        }
    }
}