using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    // masterlist enum of all the tasks
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
        ThrowOutBrokenDishes,
        GhostDirtyMirror,
        GhostDirtyFloor,
        GhostDouseFireplace
    }

    // list of tasks the player starts with, set in inspector
    public List<Task> startingTasks;
    // dynamic list of the player's current tasks
    [HideInInspector] public List<Task> taskList;

    public GhostBehavior ghost;
    public TMP_Text taskListTxt;

    private void Start()
    {
        taskList = new();
        taskListTxt = FindObjectOfType<EmptyTaskList>(true).GetComponent<TMP_Text>();
        taskListTxt.text = "";

        // fill the task list with the starting tasks
        foreach (Task task in startingTasks)
            AddTask(task);
    }

    // removes the given task from the task list
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

        ghost.RemoveTask(task);

        // gives put away dishes if clean dishes was the given task
        if (task == Task.CleanDishes)
        {
            AddTask(Task.PutAwayDishes);
            ghost.AddTask(Task.PutAwayDishes);
        }
        
        // determines which task to give if the task list is now empty
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
                FindObjectOfType<Fireplace>().SpawnEmbers();
            }
        }
    }

    // adds the given task to the task list
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