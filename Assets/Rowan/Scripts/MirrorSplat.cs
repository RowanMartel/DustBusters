using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MirrorSplat : MonoBehaviour, IPointerExitHandler
{
    [HideInInspector]
    public bool cleaned;
    int dirtLevel = 5;

    Image image;
    Mirror mirror;

    private void Start()
    {
        mirror = transform.parent.GetComponentInParent<Mirror>();
        image = GetComponent<Image>();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cleaned || !mirror.gameActive) return;

        dirtLevel--;
        image.color = new Color(image.color.r, image.color.g, image.color.g, image.color.a - .2f);
        if (dirtLevel == 0)
        {
            cleaned = true;
            mirror.CleanSplat();
        }
    }
}