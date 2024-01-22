using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    protected Image img_damageOverlay;
    protected int int_playerHealth = 3;
    protected PlayerController playerController;
    protected RWMenu menuReference;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        img_damageOverlay = FindObjectOfType<DamageOverlay>(true).GetComponent<Image>();
        menuReference = GameObject.Find("Menu").GetComponent<RWMenu>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude < 5) return;

        Pickupable pu_pickupable = collision.gameObject.GetComponent<Pickupable>();
        if (pu_pickupable == null) return;

        if (pu_pickupable.canDamagePlayer)
        {
            int_playerHealth--;
            if (int_playerHealth <= 0)
            {
                playerController.Die();
                menuReference.ShowDeathSequence();
            }
            LeanTween.alpha(img_damageOverlay.GetComponent<RectTransform>(), img_damageOverlay.color.a + 0.33f, 0.2f);         
        }
        else
        {
            LeanTween.alpha(img_damageOverlay.GetComponent<RectTransform>(), img_damageOverlay.color.a + 0.33f, 0.2f).setOnComplete(ReduceDamageOverlay);
        }
    }

    public void DamageTest()
    {
        int_playerHealth--;
        if (int_playerHealth <= 0)
        {
            playerController.Die();
            menuReference.ShowDeathSequence();
        }
        LeanTween.alpha(img_damageOverlay.GetComponent<RectTransform>(), img_damageOverlay.color.a + 0.33f, 0.2f);
    }

    private void ReduceDamageOverlay()
    {
        LeanTween.alpha(img_damageOverlay.GetComponent<RectTransform>(), img_damageOverlay.color.a - 0.33f, 1f);
    }
}
