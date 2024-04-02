using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class CleaningWater : MonoBehaviour
{
    [Tooltip("Put the splash particle effect here (see splashEffect prefab)")]
    public ParticleSystem splashEffect;
    public ParticleSystem bloodSplash;
    [Tooltip("Put all the dirty dish game objects here")]
    public List<Dish> li_dishes;

    [Tooltip("Put the clean SFX here")]
    public AudioClip ac_splash;
    AudioSource as_source;

    bool bl_bloody;

    List<CleaningWater> cleaningWaters;

    public Material mat_cleanWater;
    public Material mat_bloodyWater;
    public Renderer ren_waterVisual;

    private void Start()
    {
        as_source = GetComponent<AudioSource>();
        cleaningWaters = FindObjectsByType<CleaningWater>(FindObjectsSortMode.None).ToList();
        bl_bloody = false;
    }

    // if other is a dirty dish, clean it, then check if complete in all instances of this script
    private void OnTriggerEnter(Collider other)
    {
        GameManager.soundManager.PlayClip(ac_splash, as_source, true);
        
        //Plays the appropriate splash effect
        if (splashEffect != null)
        {
            if (bl_bloody)
            {
                bloodSplash.Play();
            }
            else
            {
                splashEffect.Play();
            }
        }

        Dish dish = other.GetComponent<Dish>();
        if (!dish || !dish.bl_dirtyDish) return;

        dish.Clean();

        foreach (CleaningWater water in cleaningWaters)
            if (water != this) water.CheckIfComplete();
        CheckIfComplete();
    }

    //Turns the sink bloody
    public void TurnBloody()
    {
        bl_bloody = true;
        ren_waterVisual.material = mat_bloodyWater;
    }

    // checks if all the dishes in the dishes list are clean, and ends the task if so
    public void CheckIfComplete()
    {
        int int_dirtyDishCount = 0;
        foreach (Dish dish in li_dishes)
            if (dish.bl_dirtyDish)
            {
                int_dirtyDishCount++;
            }
        if(int_dirtyDishCount > 0)
        {
            GameManager.taskManager.UpdateTask(li_dishes.Count - int_dirtyDishCount, li_dishes.Count, TaskManager.Task.CleanDishes);
            return;
        }
        GameManager.taskManager.CompleteTask(TaskManager.Task.CleanDishes);
    }
}