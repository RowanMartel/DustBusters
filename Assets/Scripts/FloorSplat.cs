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

    [Tooltip("Put the mopping SFX here")]
    public AudioClip cleanSFX;
    AudioSource audioSource;

    private void Start()
    {
        floorMess = transform.parent.GetComponentInParent<FloorMess>();
        image = GetComponent<Image>();

        audioSource = GetComponent<AudioSource>();
    }

    // lowers the dirt level and alpha when the mouse pointer enters the image, and cleans it if dirt level is 0
    public void OnPointerExit(PointerEventData eventData)
    {
        if (cleaned || !floorMess.gameActive) return;

        dirtLevel--;
        image.color = new Color(image.color.r, image.color.g, image.color.g, image.color.a - .2f);
        audioSource.PlayOneShot(cleanSFX);
        if (dirtLevel == 0)
        {
            cleaned = true;
            floorMess.CleanSplat();
        }
    }
}