using System.Collections.Generic;
using UnityEngine;

public class Fireplace : Interactable
{
    [Tooltip("Put the fireplace particle effect / light object here")]
    public GameObject go_fireFX;
    [Tooltip("Put the fireplace SFX here")]
    public AudioClip cl_fireplace;
    AudioSource as_source;
    public GameObject go_ember;
    public List<Transform> l_tr_emberSpawnPoints;

    private void Start()
    {
        as_source = GetComponent<AudioSource>();
        as_source.loop = true;
        as_source.clip = cl_fireplace;
    }

    // light the fireplace if the player is holding the right item
    public override void Interact()
    {
        if (GameManager.playerController.Go_heldObject == null || !GameManager.playerController.Go_heldObject.GetComponent<Pickupable>().bl_lighter) return;

        Light();
    }

    // complete the light fireplace task and start the vfx and sfx
    public void Light()
    {
        go_fireFX.SetActive(true);
        as_source.Play();

        GameManager.taskManager.CompleteTask(TaskManager.Task.LightFireplace);
    }
    // stop the vfx and sfx, and re-grant the light fireplace task
    public void UnLight()
    {
        go_fireFX.SetActive(false);
        as_source.Stop();

        if (GameManager.taskManager.li_taskList.Contains(TaskManager.Task.FindKey) ||
            GameManager.taskManager.li_taskList.Contains(TaskManager.Task.EscapeHouse))
            return;

        GameManager.taskManager.AddTask(TaskManager.Task.LightFireplace);
    }

    public void SpawnEmbers()
    {
        foreach (Transform tr in l_tr_emberSpawnPoints)
        {
            GameObject.Instantiate(go_ember, tr.position, tr.rotation);
        }
    }

}