using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    //Makes the toy in the box and checks if the task is complete
    private void OnTriggerEnter(Collider other)
    {
        Toy toy = other.GetComponent<Toy>();
        if(toy != null)
        {
            toy.bl_inBox = true;
            CheckIfComplete();
        }
        SpookyLookAtPlayer lookAt = other.GetComponent<SpookyLookAtPlayer>();
        if(lookAt != null)
        {
            lookAt.bl_canMove = false;
        }
    }

    //Makes the toy not in the box and reassign the task as needed
    private void OnTriggerExit(Collider other)
    {
        Toy toy = other.GetComponent<Toy>();
        if(toy != null)
        {
            CheckIfComplete();

            toy.bl_inBox = false;

            if (tm_taskManager.li_taskList.Contains(TaskManager.Task.PutAwayToys) ||
                tm_taskManager.li_taskList.Contains(TaskManager.Task.FindKey) ||
                tm_taskManager.li_taskList.Contains(TaskManager.Task.EscapeHouse)) return;

            tm_taskManager.AddTask(TaskManager.Task.PutAwayToys);

            CheckIfComplete();
        }
        SpookyLookAtPlayer lookAt = other.GetComponent<SpookyLookAtPlayer>();
        if (lookAt != null)
        {
            lookAt.bl_canMove = true;
        }
    }

    //Completes the task if all toys are in the box
    public void CheckIfComplete()
    {
        if (!tm_taskManager.li_taskList.Contains(TaskManager.Task.PutAwayToys)) return;

        int int_toysInBox = 0;
        foreach (Toy toy in li_toys)
        {
            if (toy.bl_inBox)
            {
                int_toysInBox++;
            }
        }

        if(int_toysInBox < li_toys.Count)
        {
            tm_taskManager.UpdateTask(int_toysInBox, TaskManager.Task.PutAwayToys);
            return;
        }

        tm_taskManager.CompleteTask(TaskManager.Task.PutAwayToys);
    }

}
