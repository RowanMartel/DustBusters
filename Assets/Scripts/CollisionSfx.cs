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
        // magnitude of the collision must be greater than or equal to 4
        if (collision.relativeVelocity.magnitude < 4) return;

        // collision must not be with the player
        if (as_source == null || ac_collide == null || collision.gameObject.CompareTag("Player")) return;

        // collision must not be with a child
        if (collision.transform.parent.gameObject == gameObject) return;

        GameManager.soundManager.PlayClip(ac_collide, as_source, true);
    }
}