using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatTrigger : MonoBehaviour
{
    //The amount of force applied to floating object
    public float floatForce;

    private void OnTriggerEnter(Collider other)
    {
        //Make Float Object stop having gravity, and push it upwards
        TestFloatObject obj = other.GetComponent<TestFloatObject>();
        if (obj != null)
        {
            Rigidbody rb = obj.gameObject.GetComponent<Rigidbody>();
            if (!obj.isFloating && obj.canFloat)
            {
                obj.isFloating = true;
                rb.useGravity = false;
                rb.AddForce(Vector3.up * floatForce);
            }
            GameManager.ghost.throwables.Add(obj.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Give Float Object it's gravity back
        TestFloatObject obj = other.GetComponent<TestFloatObject>();
        if (obj != null)
        {
            Rigidbody rb = obj.gameObject.GetComponent<Rigidbody>();
            if (obj.isFloating)
            {
                obj.isFloating = false;
                rb.useGravity = true;
            }
            GameManager.ghost.throwables.Remove(obj.gameObject);
        }
    }

}
