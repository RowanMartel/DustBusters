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

    [HideInInspector] public List<Task> taskList = new()
    {
        Task.CleanDishes,
        Task.MopFloor,
        Task.CleanMirror,
        Task.CleanCobwebs,
        Task.LightFireplace
    };

    public void CompleteTask(Task task)
    {
        taskList.Remove(task);

        if (task == Task.CleanDishes)
        {
            taskList.Add(Task.PutAwayDishes);
            SetCurrentTask(Task.PutAwayDishes);
        }

        if (taskList.Count == 0)
        {
            if (task == Task.FindKey)
            {
                taskList.Add(Task.EscapeHouse);
                SetCurrentTask(Task.EscapeHouse);
            }
            else if (task == Task.EscapeHouse)
                return;
        }
    }

    public void SetCurrentTask(Task task)
    {

    }
}