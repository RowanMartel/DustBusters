using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    // masterlist enum of all the tasks
    public enum Task
    {
        Empty,
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

    // events to notify Menu Manager of chore update and complete
    public event EventHandler<EventArgs> ChoreCompleted;
    public event EventHandler<EventArgs> ChoreUpdated;
    protected bool bl_taskListFilled;

    // Chore System variables
    public Chore currentChore;
    protected GameObject go_choreSheet;
    protected List<Chore> l_chores;
    RegionTrigger[] a_rt_regions;

    // Task System / Chore related sound variables
    private AudioSource as_taskSoundSource;
    public AudioClip ac_choreComplete;
    public AudioClip ac_choreUpdated;
    public AudioClip ac_choreAdded;

    [Tooltip("Empty,\r\n        CleanDishes,\r\n        PutAwayDishes,\r\n        MopFloor,\r\n        CleanMirror,\r\n        CleanCobwebs,\r\n        LightFireplace,\r\n        FindKey,\r\n        EscapeHouse,\r\n        ThrowOutBrokenDishes,\r\n        PutAwayBooks,\r\n        ResetBreakerBox,\r\n        GhostDirtyMirror,\r\n        GhostDirtyFloor,\r\n        GhostDouseFireplace,\r\n        PutAwayToys")]
    public int[] a_int_timesToCompleteTasks;
    int[] a_int_timesToCompleteThisRun;
    int[] a_int_timesCompletedTasks;
    public bool[] a_bl_updateOnComplete;

    // This method creates the initial list of chores and gets references to the needed objects in the MenuManager
    public void SetupChoreList()
    {
        a_int_timesCompletedTasks = new int[a_int_timesToCompleteTasks.Length];
        a_int_timesToCompleteThisRun = new int[a_int_timesToCompleteTasks.Length];
        for (int i = 0; i < a_int_timesCompletedTasks.Length; i++)
        {
            a_int_timesToCompleteThisRun[i] = a_int_timesToCompleteTasks[i];
        }


        go_choreSheet = GameObject.Find("ChoreList");
        int chores = go_choreSheet.transform.childCount;

        l_chores = new List<Chore>();

        for(int i = 0; i < chores; i++)
        {
            Chore newChore = new Chore();
            newChore.tmp_choreText = go_choreSheet.transform.GetChild(i).transform.Find("ChoreText").GetComponent<TMP_Text>();
            newChore.tmp_choreText.text = "";
            newChore.tmp_choreText.color = Color.black;
            newChore.go_box = go_choreSheet.transform.GetChild(i).transform.Find("CheckBox").gameObject;
            newChore.go_check = go_choreSheet.transform.GetChild(i).transform.Find("Check").gameObject;
            newChore.choreTask = Task.Empty;
            newChore.go_box.SetActive(false);
            newChore.go_check.SetActive(false);
            l_chores.Add(newChore);
        }

        currentChore = l_chores[0];
    }

    private void Start()
    {
        GameManager.taskManager = this;
        a_rt_regions = FindObjectsByType<RegionTrigger>(FindObjectsSortMode.None);

        as_taskSoundSource = GetComponent<AudioSource>();
    }

    public void ResetValues()
    {
        SetupChoreList();

        bl_taskListFilled = false;

        ghost = GameManager.ghost;

        li_taskList = new();

        li_tsk_completedTaskList.Clear();

        // fill the task list with the starting tasks
        foreach (Task task in li_startingTasks)
        {
            AddTask(task);
        }

        bl_taskListFilled = true;
    }

    public void UpdateTask(int int_completedAspects, Task task)
    {
        int int_index = (int)task;
        a_int_timesCompletedTasks[int_index] = int_completedAspects;
        if(currentChore.choreTask == task)
            GameManager.menuManager.UpdateCurrentChore(currentChore.tmp_choreText.text + " (" + a_int_timesCompletedTasks[int_index] + "/" + a_int_timesToCompleteThisRun[int_index] + ")");
    }

    public void UpdateTask(int int_completedAspects, int int_timesToCompleteTask, Task task)
    {
        int int_index = (int)task;
        a_int_timesCompletedTasks[int_index] = int_completedAspects;
        a_int_timesToCompleteThisRun[int_index] = int_timesToCompleteTask;
        if (currentChore.choreTask == task)
            GameManager.menuManager.UpdateCurrentChore(currentChore.tmp_choreText.text + " (" + a_int_timesCompletedTasks[int_index] + "/" + a_int_timesToCompleteThisRun[int_index] + ")");
    }

    // removes the given task from the task list and adds a strikethrough for the displayed list
    public void CompleteTask(Task task)
    {
        if (!li_taskList.Contains(task)) return;
        if (li_tsk_completedTaskList.Contains(task)) return;

        int int_index = (int)task;
        if (a_bl_updateOnComplete[int_index])
        {
            a_int_timesCompletedTasks[int_index]++;
            if (a_int_timesCompletedTasks[int_index] < a_int_timesToCompleteThisRun[int_index] && currentChore.choreTask == task)
            {
                GameManager.menuManager.UpdateCurrentChore(currentChore.tmp_choreText.text + " (" + a_int_timesCompletedTasks[int_index] + "/" + a_int_timesToCompleteThisRun[int_index] + ")");
                return;
            }
        }

        li_taskList.Remove(task);
        li_tsk_completedTaskList.Add(task);

        ghost.RemoveTask(task);

        // gives put away dishes if clean dishes was the given task
        if (task == Task.CleanDishes)
        {
            ghost.AddTask(Task.PutAwayDishes);
        }
        
        // determines which task to give if the task list is now empty
        if (li_taskList.Count == 0 || (li_taskList.Count == 1 && li_taskList.Contains(Task.ResetBreakerBox)))
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

        // this finds the task in the chore list, enables the check mark and turns the text grey
        foreach (Chore chore in l_chores)
        {
            if (chore.choreTask == task)
            {
                chore.go_check.SetActive(true);
                chore.bl_choreComplete = true;
                chore.tmp_choreText.color = Color.grey;
            }
        }

        GameManager.soundManager.PlayClip(ac_choreComplete, as_taskSoundSource, false);

        // if the completed task is the same as the "current task" highlighted on the task list, it will choose the first task on the list that is incomplete and set it to current
        if (currentChore.choreTask == task)
        {
            foreach (Chore chore in l_chores)
            {
                if (chore.bl_choreComplete == false)
                {
                    SetCurrentChore(l_chores.IndexOf(chore) + 1);
                    return;
                }
            }
        }

    }

    // adds the given task to the task list
    public void AddTask(Task task)
    {
        if (li_taskList.Contains(task)) return;
        li_taskList.Add(task);

        if (a_int_timesCompletedTasks[(int)task] > 0)
        {
            a_int_timesCompletedTasks[(int)task]--;
        }
        if (li_tsk_completedTaskList.Contains(task))
        {

            li_tsk_completedTaskList.Remove(task);
        }

        if (bl_taskListFilled)
        {
            if (ChoreUpdated != null)
                ChoreUpdated(this, new EventArgs());

            GameManager.soundManager.PlayClip(ac_choreAdded, as_taskSoundSource, false);
        }

        // If a chore is undone, this will uncheck it from the chore list and set the color to black
        foreach(Chore chore in l_chores)
        {
            if (chore.choreTask == task)
            {
                chore.go_check.SetActive(false);
                chore.bl_choreComplete = false;
                chore.tmp_choreText.color = Color.black;
                return;
            }
        }

        // If the added chore does not appear on the chores list at all, this will add it to the next available spot
        foreach(Chore chore in l_chores)
        {
            if (chore.choreTask == Task.Empty)
            {
                chore.choreTask = task;
                chore.go_box.SetActive(true);
                switch (task)
                {
                    case Task.CleanDishes:
                        chore.tmp_choreText.text = "Clean the dirty dishes in the kitchen";
                        break;
                    case Task.PutAwayDishes:
                        chore.tmp_choreText.text = "Put away the clean dishes in the kitchen";
                        break;
                    case Task.CleanCobwebs:
                        chore.tmp_choreText.text = "Dust the cobwebs in the basement";
                        break;
                    case Task.CleanMirror:
                        chore.tmp_choreText.text = "Clean the bathroom mirrors";
                        break;
                    case Task.ThrowOutBrokenDishes:
                        chore.tmp_choreText.text = "Throw out the broken dishes";
                        break;
                    case Task.EscapeHouse:
                        chore.tmp_choreText.text = "Escape the house";
                        break;
                    case Task.FindKey:
                        chore.tmp_choreText.text = "Find the front door key";
                        break;
                    case Task.LightFireplace:
                        chore.tmp_choreText.text = "Light the fireplace with the lighter";
                        break;
                    case Task.MopFloor:
                        chore.tmp_choreText.text = "Sweep the laundry room and basement";
                        break;
                    case Task.PutAwayBooks:
                        chore.tmp_choreText.text = "Tidy the books in the library";
                        break;
                    case Task.ResetBreakerBox:
                        chore.tmp_choreText.text = "Reset the breaker box in the basement";
                        break;
                    case Task.PutAwayToys:
                        chore.tmp_choreText.text = "Put away the toys in the kid's room";
                        break;
                }

                break;
            }
        }
    }

    // This changes which chore is highlighted on the list and displayed in the upper-right
    public void SetCurrentChore(int choreNumber)
    {
        if (l_chores[choreNumber - 1].bl_choreComplete == true || l_chores[choreNumber - 1].choreTask == Task.Empty) return;

        if (currentChore.tmp_choreText.color == Color.blue) currentChore.tmp_choreText.color = Color.black;

        currentChore = l_chores[choreNumber - 1];
        currentChore.tmp_choreText.color = Color.blue;

        int int_taskIndex = (int)currentChore.choreTask;

        GameManager.menuManager.UpdateCurrentChore(currentChore.tmp_choreText.text + " (" + a_int_timesCompletedTasks[int_taskIndex] + "/" + a_int_timesToCompleteThisRun[int_taskIndex] + ")");
        foreach(RegionTrigger rt_region in a_rt_regions)
        {
            rt_region.CheckObjects();
        }
    }
}

// I created a new class to help manage chores and components of the list
public class Chore
{
    public bool bl_choreComplete = false;
    public GameObject go_box;
    public GameObject go_check;
    public TMP_Text tmp_choreText;
    public TaskManager.Task choreTask;
}