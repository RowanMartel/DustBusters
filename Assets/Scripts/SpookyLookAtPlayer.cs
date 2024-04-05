using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpookyLookAtPlayer : MonoBehaviour
{
    PlayerController pc_player;
    GameObject go_player;
    Renderer ren_renderer;
    public Renderer ren_altRenderer;
    public float flt_heightOffset;
    Rigidbody rb_rigidbody;
    public bool bl_canMove;
    public bool bl_isPickupable;
    public Vector3 v3_rotationOffset;

    // Start is called before the first frame update
    void Start()
    {
        pc_player = GameManager.playerController;
        go_player = pc_player.gameObject;
        ren_renderer = GetComponent<Renderer>();
        // if the base object has no renderer, use the given one instead
        if (!ren_renderer) ren_renderer = ren_altRenderer;
        rb_rigidbody = GetComponent<Rigidbody>();
        bl_canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(ren_renderer.isVisible);
        if (bl_canMove)
        {
            if (!ren_renderer.isVisible)
            {
                transform.LookAt(go_player.transform.position);

                transform.eulerAngles += v3_rotationOffset;

                if (!bl_isPickupable)// if not pickupable, lock x and z roation
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
        }
    }
    private void OnBecameInvisible()
    {
        if (bl_canMove && bl_isPickupable)
        {
            rb_rigidbody.isKinematic = true;
        }
    }

    private void OnBecameVisible()
    {
        if (bl_canMove && bl_isPickupable)
        {
            rb_rigidbody.isKinematic = false;
        }
    }
}