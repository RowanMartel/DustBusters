using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupboardTrigger : MonoBehaviour
{
    [Tooltip("Put all the dish game objects here")]
    public List<Dish> dishes;

    private void OnTriggerEnter(Collider other)
    {
        Dish plate = other.GetComponent<Dish>();
        if (!plate || plate.dirtyDish || plate.broken) return;

        plate.inCupboard = true;

        CheckIfComplete();
    }
    private void OnTriggerExit(Collider other)
    {
        Dish plate = other.GetComponent<Dish>();
        if (!plate || plate.dirtyDish || plate.broken) return;

        plate.inCupboard = false;

        if (GameManager.taskManager.taskList.Contains(TaskManager.Task.PutAwayDishes) ||
            GameManager.taskManager.taskList.Contains(TaskManager.Task.FindKey) ||
            GameManager.taskManager.taskList.Contains(TaskManager.Task.EscapeHouse))
            return;

        GameManager.taskManager.AddTask(TaskManager.Task.PutAwayDishes);
        Debug.Log("put the dishes back");
    }

    public void CheckIfComplete()
    {
        if (!GameManager.taskManager.taskList.Contains(TaskManager.Task.PutAwayDishes)) return;
        foreach (Dish dish in dishes)
            if (!dish.inCupboard) return;

        GameManager.taskManager.CompleteTask(TaskManager.Task.PutAwayDishes);
    }
}