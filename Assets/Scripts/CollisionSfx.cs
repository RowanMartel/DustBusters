using UnityEngine;

public class CollisionSfx : MonoBehaviour
{
    [Tooltip("Attach the SFX this object makes when it collides with something")]
    public AudioClip collideSFX;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // plays the collide sfx whenever the object collides with something
    private void OnCollisionEnter(Collision collision)
    {
        if (audioSource == null || collideSFX == null) return;

        audioSource.Stop();
        audioSource.PlayOneShot(collideSFX);
    }
}