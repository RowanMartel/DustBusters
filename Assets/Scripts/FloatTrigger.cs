using System.Collections.Generic;
using UnityEngine;

public class FloatTrigger : MonoBehaviour
{
    //The amount of force applied to floating object
    public float flt_floatForce;

    List<Floatable> objects = new List<Floatable>();

    private void OnTriggerEnter(Collider cl_other)
    {
        //Make a floatable object that enters the trigger begin floating, and become throwable as an attack.
        Floatable obj = cl_other.GetComponent<Floatable>();
        if(obj != null)
        {
            objects.Add(obj);
            obj.StartFloat();
            GameManager.ghost.l_go_throwables.Add(obj.gameObject);
        }
    }

    private void OnTriggerExit(Collider cl_other)
    {
        //Make a floataoble object that leaves the trigger stop floating, and no longer be useable to attack.
        Floatable obj = cl_other.GetComponent<Floatable>();
        if (obj != null)
        {
            objects.Remove(obj);
            obj.StopFloat();
            GameManager.ghost.l_go_throwables.Remove(obj.gameObject);
        }
    }

    public void CloseTrigger()
    {
        //Stop all floating objects and turn off the trigger.
        foreach(Floatable obj in objects)
        {
            obj.StopFloat();
            GameManager.ghost.l_go_throwables.Remove(obj.gameObject);
        }
        objects.Clear();
        gameObject.SetActive(false);
    }

}
