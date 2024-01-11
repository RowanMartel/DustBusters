using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public List<Task> startingTasks;
    [HideInInspector] public List<Task> taskList;

    public GhostBehavior ghost;

    TMP_Text taskListTxt;

    private void Start()
    {
        taskList = new();
        taskListTxt = GameObject.Find("TaskList").GetComponent<TMP_Text>();
        taskListTxt.text = "";
        foreach (Task task in startingTasks)
            AddTask(task);
    }

    public void CompleteTask(Task task)
    {
        if (!taskList.Contains(task)) return;
        string text = "";
        switch (task)
        {
            case Task.CleanDishes:
                text = "\nClean dishes";
                break;
            case Task.PutAwayDishes:
                text = "\nPut away dishes";
                break;
            case Task.CleanCobwebs:
                text = "\nClean cobwebs";
                break;
            case Task.CleanMirror:
                text = "\nClean mirror";
                break;
            case Task.ThrowOutBrokenDishes:
                text = "\nThrow out broken dishes";
                break;
            case Task.EscapeHouse:
                text = "\nEscape house";
                break;
            case Task.FindKey:
                text = "\nFind key";
                break;
            case Task.LightFireplace:
                text = "\nLight fireplace";
                break;
            case Task.MopFloor:
                text = "\nMop floor";
                break;
        }
        int i = taskListTxt.text.IndexOf(text);
        taskListTxt.text = taskListTxt.text.Remove(i, text.Length);

        taskList.Remove(task);

        ghost?.RemoveTask(task);

        if (task == Task.CleanDishes)
        {
            taskList.Add(Task.PutAwayDishes);
            ghost?.AddTask(Task.PutAwayDishes);
        }
        
        if (taskList.Count == 0)
        {
            Debug.Log("task list empty");
            if (task == Task.FindKey)
                taskList.Add(Task.EscapeHouse);
            else if (task == Task.EscapeHouse)
                return;
            else
                taskList.Add(Task.FindKey);
        }
    }

    public void AddTask(Task task)
    {
        if (taskList.Contains(task)) return;
        taskList.Add(task);
        string text = "";
        switch (task)
        {
            case Task.CleanDishes:
                text = "\nClean dishes";
                break;
            case Task.PutAwayDishes:
                text = "\nPut away dishes";
                break;
            case Task.CleanCobwebs:
                text = "\nClean cobwebs";
                break;
            case Task.CleanMirror:
                text = "\nClean mirror";
                break;
            case Task.ThrowOutBrokenDishes:
                text = "\nThrow out broken dishes";
                break;
            case Task.EscapeHouse:
                text = "\nEscape house";
                break;
            case Task.FindKey:
                text = "\nFind key";
                break;
            case Task.LightFireplace:
                text = "\nLight fireplace";
                break;
            case Task.MopFloor:
                text = "\nMop floor";
                break;
        }
        taskListTxt.text += text;
    }
}