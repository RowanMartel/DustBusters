using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyChestTrigger : MonoBehaviour
{
    public List<Toy> li_toys;
    public TaskManager tm_taskManager;


    // Start is called before the first frame update
    void Start()
    {
        tm_taskManager = GameManager.taskManager;
    }

    private void OnTriggerEnter(Collider other)
    {
        Toy toy = other.GetComponent<Toy>();
        if(toy != null)
        {
            toy.bl_inBox = true;
            CheckIfComplete();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Toy toy = other.GetComponent<Toy>();
        if(toy != null)
        {
            toy.bl_inBox = false;
            //if (!tm_taskManager.li_taskList.Contains(TaskManager.Task.PutAwayToys)) tm_taskManager.AddTask(TaskManager.Task.PutAwayToys);

            if (tm_taskManager.li_taskList.Contains(TaskManager.Task.PutAwayToys) ||
                tm_taskManager.li_taskList.Contains(TaskManager.Task.FindKey) ||
                tm_taskManager.li_taskList.Contains(TaskManager.Task.EscapeHouse)) return;

            tm_taskManager.AddTask(TaskManager.Task.PutAwayToys);
        }
    }

    public void CheckIfComplete()
    {
        if (!tm_taskManager.li_taskList.Contains(TaskManager.Task.PutAwayToys)) return;
        foreach (Toy toy in li_toys)
        {
            if (!toy.bl_inBox) return;
        }
        tm_taskManager.CompleteTask(TaskManager.Task.PutAwayToys);
    }

}
