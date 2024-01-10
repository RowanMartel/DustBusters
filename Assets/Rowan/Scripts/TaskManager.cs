using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public enum Task
    {
        CleanDishes,
        PutAwayDishes,
        MopFloor,
        CleanMirror,
        CleanCobwebs,
        LightFireplace,
        FindKey,
        EscapeHouse,
        ThrowOutBrokenDishes
    }

    [HideInInspector] public Task currentTask;

    [HideInInspector] public List<Task> taskList;

    public GhostBehavior ghost;

    private void Start()
    {
        taskList = new List<Task>();
        taskList.Add(Task.CleanMirror);
    }

    public void CompleteTask(Task task)
    {
        Debug.Log(task.ToString() + " completed");

        taskList.Remove(task);

        ghost.RemoveTask(task);

        if (task == Task.CleanDishes)
        {
            taskList.Add(Task.PutAwayDishes);
            SetCurrentTask(Task.PutAwayDishes);
            ghost.AddTask(Task.PutAwayDishes);
        }

        if (taskList.Count == 0)
        {
            Debug.Log("task list empty");
            if (task == Task.FindKey)
            {
                taskList.Add(Task.EscapeHouse);
                SetCurrentTask(Task.EscapeHouse);
            }
            else if (task == Task.EscapeHouse)
                return;
            else
            {
                taskList.Add(Task.FindKey);
                Debug.Log("find the key");
            }
        }
    }

    public void SetCurrentTask(Task task)
    {

    }
}