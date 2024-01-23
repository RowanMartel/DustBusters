using UnityEngine;

public class TestFloatObject : MonoBehaviour
{
    public bool bl_canFloat;
    public bool bl_isFloating;

    Rigidbody rb;
    float flt_bobForce = 2.5f;
    float flt_bobMaxVelocity = 2.5f;
    float flt_baseHeight;
    float flt_curHeight;
    float flt_bobHeightMax = 0.75f;
    float flt_bobHeightMin = 0.5f;
    bool bl_downBob = false;

    //Setup needed variables
    private void Start()
    {
        bl_canFloat = true;
        flt_baseHeight = transform.position.y;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //Determine bobbing
        if (bl_isFloating)
        {
            flt_curHeight = transform.position.y - flt_baseHeight;
            if ((flt_curHeight < flt_bobHeightMin && bl_downBob) || (flt_curHeight > flt_bobHeightMax && !bl_downBob))
            {
                ToggleDirection();
            }

            if (bl_downBob && rb.velocity.y > -flt_bobMaxVelocity)
            {
                rb.AddForce(Vector3.down * flt_bobForce);
            }else if(!bl_downBob && rb.velocity.y < flt_bobMaxVelocity)
            {
                rb.AddForce(Vector3.up * flt_bobForce);
            }

        }
    }

    //Start floating and push upwards
    public void StartFloat()
    {
        bl_downBob = false;
        rb.useGravity = false;
        bl_isFloating = true;
        rb.AddForce(Vector3.up * flt_bobForce);
    }

    //Reset gravity
    public void StopFloat()
    {
        rb.useGravity = true;
        bl_isFloating = false;
    }

    //Switch Directions
    void ToggleDirection()
    {
        if (bl_downBob)
        {
            bl_downBob = false;
            //rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * flt_bobForce);
        }
        else
        {
            bl_downBob = true;
            //rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.down * flt_bobForce);
        }
    }
}
