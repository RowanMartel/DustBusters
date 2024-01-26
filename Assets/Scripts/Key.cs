public class Key : Pickupable
{
    // completes the find key task when picked up by the player, only if they have that task
    public override void Interact()
    {
        if (GameManager.taskManager.li_taskList.Contains(TaskManager.Task.FindKey))
            GameManager.taskManager.CompleteTask(TaskManager.Task.FindKey);
    }
}