using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatTrigger : MonoBehaviour
{

    public float floatForce;
    public GhostBehavior ghost;

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
            ghost.throwables.Add(obj.gameObject);
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
            ghost.throwables.Remove(obj.gameObject);
        }
        catch
        {

        }
    }

}
