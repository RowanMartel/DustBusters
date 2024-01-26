using System.Collections.Generic;
using UnityEngine;

public class FloatTrigger : MonoBehaviour
{
    //The amount of force applied to floating object
    public float flt_floatForce;

    List<Floatable> objects = new List<Floatable>();

    private void OnTriggerEnter(Collider cl_other)
    {
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
        foreach(Floatable obj in objects)
        {
            obj.StopFloat();
            GameManager.ghost.l_go_throwables.Remove(obj.gameObject);
        }
        objects.Clear();
        gameObject.SetActive(false);
    }

}
