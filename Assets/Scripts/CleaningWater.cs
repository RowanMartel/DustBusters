using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CleaningWater : MonoBehaviour
{
    [Tooltip("Put all the dirty dish game objects here")]
    public List<Dish> dishes;

    [Tooltip("Put the clean SFX here")]
    public AudioClip splashSFX;
    AudioSource audioSource;

    List<CleaningWater> cleaningWaters;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        cleaningWaters = FindObjectsByType<CleaningWater>(FindObjectsSortMode.None).ToList();
    }

    // if other is a dirty dish, clean it, then check if complete in all instances of this script
    private void OnTriggerEnter(Collider other)
    {
        audioSource.PlayOneShot(splashSFX);

        Dish dish = other.GetComponent<Dish>();
        if (!dish || !dish.dirtyDish) return;

        dish.Clean();

        foreach (CleaningWater water in cleaningWaters)
            if (water != this) water.CheckIfComplete();
        CheckIfComplete();
    }

    // checks if all the dishes in the dishes list are clean, and ends the task if so
    public void CheckIfComplete()
    {
        foreach (Dish dish in dishes)
            if (dish.dirtyDish) return;

        GameManager.taskManager.CompleteTask(TaskManager.Task.CleanDishes);
    }
}