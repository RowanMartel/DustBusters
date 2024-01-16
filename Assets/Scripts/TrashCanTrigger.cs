using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrashCanTrigger : MonoBehaviour
{
    [HideInInspector] public List<Dish> dishes;
    List<TrashCanTrigger> triggers;

    private void Start()
    {
        triggers = FindObjectsByType<TrashCanTrigger>(FindObjectsSortMode.None).ToList();
    }

    // marks the entering broken plate as being in the trash
    private void OnTriggerEnter(Collider other)
    {
        Dish plate = other.GetComponent<Dish>();
        if (!plate || !plate.broken) return;

        plate.inTrash = true;

        foreach (TrashCanTrigger trigger in triggers)
            if (trigger != this) trigger.CheckIfComplete();
        CheckIfComplete();
    }
    // marks the exiting broken plate as no longer being in the trash and re-grants the task
    private void OnTriggerExit(Collider other)
    {
        Dish plate = other.GetComponent<Dish>();
        if (!plate || !plate.broken) return;

        plate.inTrash = false;

        if (GameManager.taskManager.taskList.Contains(TaskManager.Task.ThrowOutBrokenDishes) ||
            GameManager.taskManager.taskList.Contains(TaskManager.Task.FindKey) ||
            GameManager.taskManager.taskList.Contains(TaskManager.Task.EscapeHouse))
            return;
        GameManager.taskManager.AddTask(TaskManager.Task.ThrowOutBrokenDishes);
    }

    public void CheckIfComplete()
    {
        foreach (Dish dish in dishes)
            if (!dish.inTrash) return;
        GameManager.taskManager.CompleteTask(TaskManager.Task.ThrowOutBrokenDishes);
    }
}