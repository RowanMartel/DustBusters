using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(GameObject button)
    {
        LeanTween.scale(button, new Vector3 (1.1f, 1.1f, 1.1f), .1f).setIgnoreTimeScale(true);
    }

    public void OnPointerExit(GameObject button)
    {
        LeanTween.scale(button, new Vector3(1, 1, 1), .1f).setIgnoreTimeScale(true);
    }

    public void OnPointerEnterSlider(GameObject slider)
    {
        LeanTween.scale(slider, new Vector3(1.075f, 1.075f, 1.075f), .1f).setIgnoreTimeScale(true);
    }

    public void OnPointerExitSlider(GameObject slider)
    {
        LeanTween.scale(slider, new Vector3(1, 1, 1), .1f).setIgnoreTimeScale(true);
    }
}
