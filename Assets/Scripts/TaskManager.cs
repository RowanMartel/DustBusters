using System;
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
        PutAwayBooks,
        ResetBreakerBox,
        GhostDirtyMirror,
        GhostDirtyFloor,
        GhostDouseFireplace,
        PutAwayToys
    }

    // list of tasks the player starts with, set in inspector
    public List<Task> li_startingTasks;
    // dynamic list of the player's current tasks
    [HideInInspector] public List<Task> li_taskList;
    [HideInInspector] public List<Task> li_tsk_completedTaskList;

    public GhostBehavior ghost;
    public TMP_Text tmp_taskListTxt;

    // events to notify Menu Manager of chore update and complete
    public event EventHandler<EventArgs> ChoreCompleted;
    public event EventHandler<EventArgs> ChoreUpdated;
    protected bool bl_taskListFilled;

    private void Awake()
    {
        GameManager.taskManager = this;
    }

    public void ResetValues()
    {
        bl_taskListFilled = false;

        ghost = GameManager.ghost;

        li_taskList = new();
        tmp_taskListTxt = FindObjectOfType<EmptyTaskList>(true).GetComponent<TMP_Text>();
        tmp_taskListTxt.text = "";

        li_tsk_completedTaskList.Clear();

        // fill the task list with the starting tasks
        foreach (Task task in li_startingTasks)
        {
            AddTask(task);
        }

        bl_taskListFilled = true;
    }

    // removes the given task from the task list and adds a strikethrough for the displayed list
    public void CompleteTask(Task task)
    {
        if (!li_taskList.Contains(task)) return;
        if (li_tsk_completedTaskList.Contains(task)) return;

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
            case Task.PutAwayBooks:
                text = "\nPut the books back on the shelf";
                break;
            case Task.ResetBreakerBox:
                text = "\nReset the breaker box";
                break;
            case Task.PutAwayToys:
                text = "\nPut away the toys";
                break;
        }
        int i = tmp_taskListTxt.text.IndexOf(text);
        tmp_taskListTxt.text = tmp_taskListTxt.text.Remove(i, text.Length);

        //Adds a strikethrough to a completed task
        tmp_taskListTxt.text = tmp_taskListTxt.text.Insert(i, "<s>" + text + "</s>");

        li_taskList.Remove(task);
        li_tsk_completedTaskList.Add(task);

        ghost.RemoveTask(task);

        // gives put away dishes if clean dishes was the given task
        if (task == Task.CleanDishes)
        {
            AddTask(Task.PutAwayDishes);
            ghost.AddTask(Task.PutAwayDishes);
        }
        
        // determines which task to give if the task list is now empty
        if (li_taskList.Count == 0)
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

        if (ChoreCompleted != null)
            ChoreCompleted(this, new EventArgs());

        Debug.Log("Chore Completed: " + task.ToString());
    }

    // adds the given task to the task list
    public void AddTask(Task task)
    {
        if (li_taskList.Contains(task)) return;
        li_taskList.Add(task);
        string str_text = "";
        switch (task)
        {
            case Task.CleanDishes:
                str_text = "\nClean the dirty dishes";
                break;
            case Task.PutAwayDishes:
                str_text = "\nPut away the dishes on the counter";
                break;
            case Task.CleanCobwebs:
                str_text = "\nClean away the cobwebs in the foyer";
                break;
            case Task.CleanMirror:
                str_text = "\nClean the bathroom mirror";
                break;
            case Task.ThrowOutBrokenDishes:
                str_text = "\nThrow out the broken dishes";
                break;
            case Task.EscapeHouse:
                str_text = "\nEscape the house";
                break;
            case Task.FindKey:
                str_text = "\nFind the front door key";
                break;
            case Task.LightFireplace:
                str_text = "\nLight the fireplace";
                break;
            case Task.MopFloor:
                str_text = "\nMop the laundry room floor";
                break;
            case Task.PutAwayBooks:
                str_text = "\nPut the books back on the shelf";
                break;
            case Task.ResetBreakerBox:
                str_text = "\nReset the breaker box";
                break;
            case Task.PutAwayToys:
                str_text = "\nPut away the toys";
                break;
        }

        if (li_tsk_completedTaskList.Contains(task))
        {
            //Remove strikethrough from an uncompleted task
            string str_strikethroughText = "<s>" + str_text + "</s>";

            int i = tmp_taskListTxt.text.IndexOf(str_strikethroughText);

            tmp_taskListTxt.text = tmp_taskListTxt.text.Remove(i, str_strikethroughText.Length);

            tmp_taskListTxt.text = tmp_taskListTxt.text.Insert(i, str_text);

            li_tsk_completedTaskList.Remove(task);
        }
        else
        {
            tmp_taskListTxt.text += str_text;
        }

        if (bl_taskListFilled)
        {
            if (ChoreUpdated != null)
                ChoreUpdated(this, new EventArgs());
        }

        Debug.Log("Chore Updated: " + task.ToString());
    }
}