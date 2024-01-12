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

    public TMP_Text taskListTxt;

    private void Start()
    {
        taskList = new();
        taskListTxt = FindObjectOfType<EmptyTaskList>(true).GetComponent<TMP_Text>();
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
                text = "\nClean the dirty dishes";
                break;
            case Task.PutAwayDishes:
                text = "\nPut away the dishes on the counter";
                break;
            case Task.CleanCobwebs:
                text = "\nClean away the cobwebs in the foyer";
                break;
            case Task.CleanMirror:
                text = "\nClean the bathroom mirror";
                break;
            case Task.ThrowOutBrokenDishes:
                text = "\nThrow out the broken dishes";
                break;
            case Task.EscapeHouse:
                text = "\nEscape the house";
                break;
            case Task.FindKey:
                text = "\nFind the front door key";
                break;
            case Task.LightFireplace:
                text = "\nLight the fireplace";
                break;
            case Task.MopFloor:
                text = "\nMop the laundry room floor";
                break;
        }
        int i = taskListTxt.text.IndexOf(text);
        taskListTxt.text = taskListTxt.text.Remove(i, text.Length);

        taskList.Remove(task);

        ghost?.RemoveTask(task);

        if (task == Task.CleanDishes)
        {
            AddTask(Task.PutAwayDishes);
            ghost?.AddTask(Task.PutAwayDishes);
        }
        
        if (taskList.Count == 0)
        {
            if (task == Task.FindKey)
                AddTask(Task.EscapeHouse);
            else if (task == Task.EscapeHouse)
                return;
            else
            {
                AddTask(Task.FindKey);
                FindObjectOfType<GhostBehavior>().EnterEndGame();
            }
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
                text = "\nClean the dirty dishes";
                break;
            case Task.PutAwayDishes:
                text = "\nPut away the dishes on the counter";
                break;
            case Task.CleanCobwebs:
                text = "\nClean away the cobwebs in the foyer";
                break;
            case Task.CleanMirror:
                text = "\nClean the bathroom mirror";
                break;
            case Task.ThrowOutBrokenDishes:
                text = "\nThrow out the broken dishes";
                break;
            case Task.EscapeHouse:
                text = "\nEscape the house";
                break;
            case Task.FindKey:
                text = "\nFind the front door key";
                break;
            case Task.LightFireplace:
                text = "\nLight the fireplace";
                break;
            case Task.MopFloor:
                text = "\nMop the laundry room floor";
                break;
        }
        taskListTxt.text += text;
    }
}