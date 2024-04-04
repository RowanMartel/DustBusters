using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpookyLookAtPlayer : MonoBehaviour
{

    PlayerController pc_player;
    GameObject go_player;
    Renderer ren_renderer;
    public float flt_heightOffset;
    Rigidbody rb_rigidbody;
    public bool bl_canMove;

    // Start is called before the first frame update
    void Start()
    {
        pc_player = GameManager.playerController;
        go_player = pc_player.gameObject;
        ren_renderer = GetComponent<Renderer>();
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
            }
        }
    }
    private void OnBecameInvisible()
    {
        if (bl_canMove)
        {
            rb_rigidbody.isKinematic = true;
        }
    }

    private void OnBecameVisible()
    {
        if (bl_canMove)
        {
            rb_rigidbody.isKinematic = false;
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<ToyChestTrigger>() != null)
        {
            Debug.Log("AA");
            bl_canMove = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ToyChestTrigger>() != null)
        {
            bl_canMove = true;
        }
    }*/

}
