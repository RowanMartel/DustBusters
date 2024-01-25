using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrashCanTrigger : MonoBehaviour
{
    [HideInInspector] public List<Dish> li_dishes;
    List<TrashCanTrigger> li_triggers;

    private void Start()
    {
        li_triggers = FindObjectsByType<TrashCanTrigger>(FindObjectsSortMode.None).ToList();
    }

    // marks the entering broken plate as being in the trash
    private void OnTriggerEnter(Collider other)
    {
        Dish plate = other.GetComponent<Dish>();
        if (!plate || !plate.bl_broken) return;

        plate.inTrash = true;

        foreach (TrashCanTrigger trigger in li_triggers)
            if (trigger != this) trigger.CheckIfComplete();
        CheckIfComplete();
    }
    // marks the exiting broken plate as no longer being in the trash and re-grants the task
    private void OnTriggerExit(Collider other)
    {
        Dish plate = other.GetComponent<Dish>();
        if (!plate || !plate.bl_broken) return;

        plate.inTrash = false;

        if (GameManager.taskManager.li_taskList.Contains(TaskManager.Task.ThrowOutBrokenDishes) ||
            GameManager.taskManager.li_taskList.Contains(TaskManager.Task.FindKey) ||
            GameManager.taskManager.li_taskList.Contains(TaskManager.Task.EscapeHouse))
            return;
        GameManager.taskManager.AddTask(TaskManager.Task.ThrowOutBrokenDishes);
    }

    public void CheckIfComplete()
    {
        foreach (Dish dish in li_dishes)
        {
            if (!dish.inTrash)
            {
                if (GameManager.taskManager.li_taskList.Contains(TaskManager.Task.ThrowOutBrokenDishes) ||
                    GameManager.taskManager.li_taskList.Contains(TaskManager.Task.FindKey) ||
                    GameManager.taskManager.li_taskList.Contains(TaskManager.Task.EscapeHouse))
                    return;
                GameManager.taskManager.AddTask(TaskManager.Task.ThrowOutBrokenDishes);
                return;
            }
        }
        GameManager.taskManager.CompleteTask(TaskManager.Task.ThrowOutBrokenDishes);
    }
}