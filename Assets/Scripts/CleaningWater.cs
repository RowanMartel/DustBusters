using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaningWater : MonoBehaviour
{
    [Tooltip("Put all the dirty dish game objects here")]
    public List<Dish> dishes;

    [Tooltip("Put the clean SFX here")]
    public AudioClip cleanSFX;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Dish dish = other.GetComponent<Dish>();
        if (!dish || !dish.dirtyDish) return;

        dish.Clean();

        CheckIfComplete();
    }

    public void CheckIfComplete()
    {
        foreach (Dish dish in dishes)
            if (dish.dirtyDish) return;

        audioSource.PlayOneShot(cleanSFX);
        GameManager.taskManager.CompleteTask(TaskManager.Task.CleanDishes);
    }
}