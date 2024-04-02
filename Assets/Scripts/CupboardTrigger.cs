using System.Collections.Generic;
using UnityEngine;

public class CupboardTrigger : MonoBehaviour
{
    [Tooltip("Put all the dish game objects here")]
    public List<Dish> li_dishes;

    // counts the dish as being in the cupboard, then checks if complete
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Cupboard Trigger");
        Dish dish = other.GetComponent<Dish>();
        if (!dish || dish.bl_dirtyDish || dish.bl_broken) return;

        dish.inCupboard = true;
        Debug.Log("Accepted Dish");

        CheckIfComplete();
    }

    // counts the dish as no longer in the cupboard and grants the task again
    private void OnTriggerExit(Collider other)
    {
        Dish plate = other.GetComponent<Dish>();
        if (!plate || plate.bl_dirtyDish || plate.bl_broken) return;

        plate.inCupboard = false;

        CheckIfComplete();

        if (GameManager.taskManager.li_taskList.Contains(TaskManager.Task.PutAwayDishes) ||
            GameManager.taskManager.li_taskList.Contains(TaskManager.Task.FindKey) ||
            GameManager.taskManager.li_taskList.Contains(TaskManager.Task.EscapeHouse) ||
            GameManager.taskManager.li_taskList.Contains(TaskManager.Task.CleanDishes))
            return;

        GameManager.taskManager.AddTask(TaskManager.Task.PutAwayDishes);
    }

    // checks if all the dishes in the dishes list are in the cupboard, then completes the task if so
    public void CheckIfComplete()
    {
        if (!GameManager.taskManager.li_taskList.Contains(TaskManager.Task.PutAwayDishes)) return;

        int int_dishesInCupboard = 0;
        foreach (Dish dish in li_dishes)
        {
            if (dish.inCupboard && !dish.bl_dirtyDish)
            {
                int_dishesInCupboard++;
            }
        }

        if (int_dishesInCupboard < li_dishes.Count)
        {
            Debug.Log(int_dishesInCupboard);
            GameManager.taskManager.UpdateTask(int_dishesInCupboard, li_dishes.Count, TaskManager.Task.PutAwayDishes);
            return;
        }

        GameManager.taskManager.CompleteTask(TaskManager.Task.PutAwayDishes);
    }
}