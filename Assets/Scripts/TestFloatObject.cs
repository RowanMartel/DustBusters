using UnityEngine;

public class TestFloatObject : MonoBehaviour
{
    public bool bl_canFloat;
    public bool bl_isFloating;

    Rigidbody rb;
    float flt_baseGravity = 1;
    float flt_bobForce = 50;
    float flt_baseHeight;
    float flt_curHeight;
    float flt_bobHeightMax = 1.25f;
    float flt_bobHeightMin = 0.75f;
    bool bl_downBob = false;

    private void Start()
    {
        bl_canFloat = true;
        flt_baseHeight = transform.position.y;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (bl_isFloating)
        {
            flt_curHeight = transform.position.y - flt_baseHeight;
            if ((flt_curHeight < flt_bobHeightMin && bl_downBob) || (flt_curHeight > flt_bobHeightMax && !bl_downBob))
            {
                ToggleDirection();
            }
        }
    }

    public void StartFloat()
    {
        bl_downBob = false;
        rb.useGravity = false;
        bl_isFloating = true;
        rb.AddForce(Vector3.up * flt_bobForce);
        Debug.Log("A");
    }

    public void StopFloat()
    {
        rb.useGravity = true;
        bl_isFloating = false;
    }

    void ToggleDirection()
    {
        if (bl_downBob)
        {
            bl_downBob = false;
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up * flt_bobForce);
        }
        else
        {
            bl_downBob = true;
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.down * flt_bobForce);
        }
    }
}
