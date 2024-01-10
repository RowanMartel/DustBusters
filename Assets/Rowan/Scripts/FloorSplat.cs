/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FloorSplat : MonoBehaviour, IPointerExitHandler
{
    [HideInInspector]
    public bool cleaned;
    int dirtLevel = 5;

    Image image;
    FloorMess floorMess;

    private void Start()
    {
        floorMess = transform.parent.GetComponentInParent<FloorMess>();
        image = GetComponent<Image>();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (cleaned || !floorMess.gameActive) return;

        dirtLevel--;
        image.color = new Color(image.color.r, image.color.g, image.color.g, image.color.a - .2f);
        if (dirtLevel == 0)
        {
            cleaned = true;
            floorMess.CleanSplat();
        }
    }
}*/