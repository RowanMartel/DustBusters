using UnityEngine;

public class CollisionSfx : MonoBehaviour
{
    [Tooltip("Attach the SFX this object makes when it collides with something")]
    public AudioClip ac_collide;
    AudioSource as_source;

    void Start()
    {
        as_source = GetComponent<AudioSource>();
    }

    // plays the collide sfx whenever the object collides with something
    private void OnCollisionEnter(Collision collision)
    {
        if (as_source == null || ac_collide == null || collision.gameObject.CompareTag("Player")) return;

        GameManager.soundManager.PlayClip(ac_collide, as_source);
    }
}