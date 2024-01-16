using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    Image damageOverlay;
    Image deathMessage;
    public int health;
    public PlayerController playerController;

    RWMenu menuReference;

    // Start is called before the first frame update
    void Start()
    {
        damageOverlay = FindObjectOfType<DamageOverlay>(true).GetComponent<Image>();
        deathMessage = FindObjectOfType<DeathMessage>(true).GetComponent<Image>();
        menuReference = GameObject.Find("Menu").GetComponent<RWMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude < 5) return;

        Pickupable pickupable = collision.gameObject.GetComponent<Pickupable>();
        if (pickupable == null) return;

        if (pickupable.canDamagePlayer)
        {
            health--;
            if (health <= 0)
            {
                playerController.Die();
                LeanTween.alpha(deathMessage.GetComponent<RectTransform>(), 1, 1f).setOnComplete(menuReference.ShowDeathScreen);
            }

            LeanTween.alpha(damageOverlay.GetComponent<RectTransform>(), damageOverlay.color.a + 0.33f, 0.2f);         
        }
        else
        {
            LeanTween.alpha(damageOverlay.GetComponent<RectTransform>(), damageOverlay.color.a + 0.33f, 0.2f).setOnComplete(ReduceDamageOverlay);
        }
    }

    private void ReduceDamageOverlay()
    {
        LeanTween.alpha(damageOverlay.GetComponent<RectTransform>(), damageOverlay.color.a - 0.33f, 1f);
    }
}
