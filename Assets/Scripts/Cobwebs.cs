using UnityEngine;

public class Cobwebs : MonoBehaviour
{
    [Tooltip("Put the amount of cobwebs the player needs to clean here")]
    public int cobwebs;

    // ticks down cobwebs int, then ends the task if complete
    public void CleanWeb()
    {
        cobwebs--;
        if (cobwebs <= 0)
            GameManager.taskManager.CompleteTask(TaskManager.Task.CleanCobwebs);
    }
}