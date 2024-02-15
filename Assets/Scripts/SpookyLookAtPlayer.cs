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

    // Start is called before the first frame update
    void Start()
    {
        pc_player = GameManager.playerController;
        go_player = pc_player.gameObject;
        ren_renderer = GetComponent<Renderer>();
        rb_rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(ren_renderer.isVisible);
        if (!ren_renderer.isVisible)
        {
            transform.LookAt(go_player.transform.position);
        }
    }

    private void OnBecameInvisible()
    {
        rb_rigidbody.isKinematic = true;
    }

    private void OnBecameVisible()
    {
        rb_rigidbody.isKinematic = false;
    }

}
