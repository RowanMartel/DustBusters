using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatTrigger : MonoBehaviour
{

    public float floatForce;

    private void OnTriggerEnter(Collider other)
    {
        try
        {
            TestFloatObject obj = other.GetComponent<TestFloatObject>();
            Rigidbody rb = obj.gameObject.GetComponent<Rigidbody>();
            if(!obj.isFloating && obj.canFloat)
            {
                obj.isFloating = true;
                rb.useGravity = false;
                rb.AddForce(Vector3.up * floatForce);
            }
        }
        catch
        {

        }
    }

    private void OnTriggerExit(Collider other)
    {
        try
        {
            TestFloatObject obj = other.GetComponent<TestFloatObject>();
            Rigidbody rb = obj.gameObject.GetComponent<Rigidbody>();
            if (obj.isFloating)
            {
                obj.isFloating = false;
                rb.useGravity = true;
            }
        }
        catch
        {

        }
    }

}
