using UnityEngine;

public class Cobwebs : MonoBehaviour
{
    [Tooltip("Put the amount of cobwebs the player needs to clean here")]
    public int int_cobwebs;
    public int int_startingCobwebs;

    // ticks down cobwebs int, then ends the task if complete
    public void CleanWeb()
    {
        int_cobwebs--;
        GameManager.taskManager.UpdateTask(int_startingCobwebs - int_cobwebs, TaskManager.Task.CleanCobwebs);
        if (int_cobwebs <= 0)
            GameManager.taskManager.CompleteTask(TaskManager.Task.CleanCobwebs);
    }
}