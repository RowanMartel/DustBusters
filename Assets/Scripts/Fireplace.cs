using System.Collections.Generic;
using UnityEngine;

public class Fireplace : Interactable
{
    [Tooltip("Put the fireplace particle effect / light object here")]
    public GameObject go_fireFX;
    public ParticleSystem ps_fire;
    public ParticleSystem ps_smoke;
    public float flt_fireEmissionRate;
    public float flt_smokeEmissionRate;
    [Tooltip("Put the fireplace SFX here")]
    public AudioClip cl_fireplace;
    AudioSource as_source;
    public GameObject go_ember;
    public List<Transform> l_tr_emberSpawnPoints;
    bool bl_lit;

    private void Start()
    {
        ps_fire.Stop();

        ps_smoke.Stop();

        bl_lit = false;
        as_source = GetComponent<AudioSource>();
        as_source.loop = true;
        as_source.clip = cl_fireplace;
        cl_fireplace.LoadAudioData();
    }

    // light the fireplace if the player is holding the right item
    public override void Interact()
    {
        if (GameManager.playerController.Go_heldObject == null || !GameManager.playerController.Go_heldObject.GetComponent<Pickupable>().bl_lighter || bl_lit == true) return;
        Light();
    }

    // complete the light fireplace task and start the vfx and sfx
    public void Light()
    {
        //Activate light and audio
        go_fireFX.SetActive(true);
        as_source.Play();

        //Activate particles
        ps_fire.Play();
        ps_fire.Clear();
        ps_smoke.Play();
        ps_smoke.Clear();

        bl_lit = true;

        GameManager.taskManager.CompleteTask(TaskManager.Task.LightFireplace);
    }

    // stop the vfx and sfx, and re-grant the light fireplace task
    public void UnLight()
    {
        //Stop light and audio
        go_fireFX.SetActive(false);
        as_source.Stop();

        //Stop particles
        ps_fire.Stop();
        ps_fire.Clear();
        ps_smoke.Stop();
        ps_smoke.Clear();

        bl_lit = false;

        if (GameManager.taskManager.li_taskList.Contains(TaskManager.Task.FindKey) ||
            GameManager.taskManager.li_taskList.Contains(TaskManager.Task.EscapeHouse))
            return;

        GameManager.taskManager.AddTask(TaskManager.Task.LightFireplace);
    }

    //Spawns Embers for ghost to use
    public void SpawnEmbers()
    {
        foreach (Transform tr in l_tr_emberSpawnPoints)
        {
            GameObject.Instantiate(go_ember, tr.position, tr.rotation);
        }
    }

}