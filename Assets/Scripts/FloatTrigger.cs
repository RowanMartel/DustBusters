using UnityEngine;

public class FloatTrigger : MonoBehaviour
{
    //The amount of force applied to floating object
    public float flt_floatForce;

    private void OnTriggerEnter(Collider cl_other)
    {
        //Make Float Object stop having gravity, and push it upwards
        TestFloatObject obj = cl_other.GetComponent<TestFloatObject>();
        if (obj != null)
        {
            Rigidbody rb = obj.gameObject.GetComponent<Rigidbody>();
            if (!obj.bl_isFloating && obj.bl_canFloat)
            {
                obj.bl_isFloating = true;
                rb.useGravity = false;
                rb.AddForce(Vector3.up * flt_floatForce);
            }
            GameManager.ghost.l_go_throwables.Add(obj.gameObject);
        }
    }

    private void OnTriggerExit(Collider cl_other)
    {
        //Give Float Object it's gravity back
        TestFloatObject obj = cl_other.GetComponent<TestFloatObject>();
        if (obj != null)
        {
            Rigidbody rb = obj.gameObject.GetComponent<Rigidbody>();
            if (obj.bl_isFloating)
            {
                obj.bl_isFloating = false;
                rb.useGravity = true;
            }
            GameManager.ghost.l_go_throwables.Remove(obj.gameObject);
        }
    }

}
