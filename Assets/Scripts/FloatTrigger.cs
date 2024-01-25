using System.Collections.Generic;
using UnityEngine;

public class FloatTrigger : MonoBehaviour
{
    //The amount of force applied to floating object
    public float flt_floatForce;

    List<TestFloatObject> objects = new List<TestFloatObject>();

    private void OnTriggerEnter(Collider cl_other)
    {
        TestFloatObject obj = cl_other.GetComponent<TestFloatObject>();
        if(obj != null)
        {
            objects.Add(obj);
            obj.StartFloat();
            GameManager.ghost.l_go_throwables.Add(obj.gameObject);
        }
    }

    private void OnTriggerExit(Collider cl_other)
    {
        TestFloatObject obj = cl_other.GetComponent<TestFloatObject>();
        if (obj != null)
        {
            objects.Remove(obj);
            obj.StopFloat();
            GameManager.ghost.l_go_throwables.Remove(obj.gameObject);
        }
    }

    public void CloseTrigger()
    {
        foreach(TestFloatObject obj in objects)
        {
            obj.StopFloat();
            GameManager.ghost.l_go_throwables.Remove(obj.gameObject);
        }
        objects.Clear();
        gameObject.SetActive(false);
    }

}
