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

    [Tooltip("Put the dusting SFX here")]
    public AudioClip cleanSFX;
    AudioSource audioSource;

    private void Start()
    {
        mirror = transform.parent.GetComponentInParent<Mirror>();
        image = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
    }

    // lowers the dirt level and alpha when the mouse pointer enters the image, and cleans it if dirt level is 0
    public void OnPointerExit(PointerEventData eventData)
    {
        if (cleaned || !mirror.gameActive) return;

        dirtLevel--;
        image.color = new Color(image.color.r, image.color.g, image.color.g, image.color.a - .2f);
        audioSource.PlayOneShot(cleanSFX);
        if (dirtLevel == 0)
        {
            cleaned = true;
            mirror.CleanSplat();
        }
    }
}