using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    Image damageOverlay;
    public int health;
    public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        damageOverlay = GameObject.Find("DamageOverlay").GetComponent<Image>();
        // damageOverlay.color = new Color(damageOverlay.color.r, damageOverlay.color.g, damageOverlay.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude < 10) return;

        Pickupable pickupable = collision.gameObject.GetComponent<Pickupable>();
        if (pickupable == null) return;

        if (pickupable.canDamagePlayer)
        {
            health--;
            if (health <= 0) playerController.Die();

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
