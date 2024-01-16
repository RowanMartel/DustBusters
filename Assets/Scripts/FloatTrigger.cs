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
            GameManager.ghost.throwables.Add(obj.gameObject);
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
            GameManager.ghost.throwables.Remove(obj.gameObject);
        }
        catch
        {

        }
    }

}
