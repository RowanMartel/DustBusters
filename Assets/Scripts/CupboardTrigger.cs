using System.Collections.Generic;
using UnityEngine;

public class CupboardTrigger : MonoBehaviour
{
    [Tooltip("Put all the dish game objects here")]
    public List<Dish> dishes;

    // counts the dish as being in the cupboard, then checks if complete
    private void OnTriggerEnter(Collider other)
    {
        Dish dish = other.GetComponent<Dish>();
        if (!dish || dish.dirtyDish || dish.broken) return;

        dish.inCupboard = true;

        CheckIfComplete();
    }
    // counts the dish as no longer in the cupboard and grants the task again
    private void OnTriggerExit(Collider other)
    {
        Dish plate = other.GetComponent<Dish>();
        if (!plate || plate.dirtyDish || plate.broken) return;

        plate.inCupboard = false;

        if (GameManager.taskManager.taskList.Contains(TaskManager.Task.PutAwayDishes) ||
            GameManager.taskManager.taskList.Contains(TaskManager.Task.FindKey) ||
            GameManager.taskManager.taskList.Contains(TaskManager.Task.EscapeHouse) ||
            GameManager.taskManager.taskList.Contains(TaskManager.Task.CleanDishes))
            return;

        GameManager.taskManager.AddTask(TaskManager.Task.PutAwayDishes);
    }

    // checks if all the dishes in the dishes list are in the cupboard, then completes the task if so
    public void CheckIfComplete()
    {
        if (!GameManager.taskManager.taskList.Contains(TaskManager.Task.PutAwayDishes)) return;
        foreach (Dish dish in dishes)
            if (!dish.inCupboard) return;

        GameManager.taskManager.CompleteTask(TaskManager.Task.PutAwayDishes);
    }
}