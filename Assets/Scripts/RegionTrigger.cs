using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RegionTrigger : MonoBehaviour
{

    NavMeshObstacle nav_obstacle;
    GhostBehavior gb_ghost;
    PlayerController pc_player;
    MirrorMovement[] a_mirrorMovements;
    public Image img_itemIndicator;
    List<Pickupable> l_pu_pickupables = new List<Pickupable>();
    [Tooltip("0:Lighter 1:Duster 2:Mop 3:Key")]
    public Sprite[] a_spt_itemSprites;

    private void Start()
    {
        nav_obstacle = GetComponent<NavMeshObstacle>();
        gb_ghost = GameManager.ghost;
        pc_player = GameManager.playerController;
        a_mirrorMovements = GameObject.FindObjectsOfType<MirrorMovement>();
        CheckObjects();
        //img_itemIndicator = transform.GetChild(0).GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        CheckObjects();
    }

    //Assigns current region to characters who enter the region
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pc_player.go_curRegion = gameObject;

            if (gb_ghost.bl_hiding && gb_ghost.go_curRegion != pc_player.go_curRegion && gb_ghost.int_curAggressionLevel < 3)
            {
                nav_obstacle.enabled = true;
            }

            foreach(MirrorMovement mir in a_mirrorMovements)
            {
                if (mir.a_go_regions.Contains(gameObject))
                {
                    mir.ActivateCam();
                }
                else
                {
                    mir.DeactivateCam();
                }
            }

        }else if (other.CompareTag("Ghost"))
        {
            GameManager.ghost.go_curRegion = gameObject;
        }else if (other.GetComponent<Pickupable>() != null)
        {
            Pickupable pu_other = other.GetComponent<Pickupable>();
            l_pu_pickupables.Add(pu_other);
            CheckObjects();
        }
    }

    //Turns off the nav mesh obstacle if needed
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && nav_obstacle.enabled)
        {
            nav_obstacle.enabled = false;
        }
        else if (other.GetComponent<Pickupable>() != null)
        {
            Pickupable pu_other = other.GetComponent<Pickupable>();
            while (l_pu_pickupables.Contains(pu_other))
            {
                Debug.Log("removing " + pu_other.name);
                l_pu_pickupables.Remove(pu_other);
            }
            CheckObjects();
        }
    }

    public void CheckObjects()
    {
        Chore currentChore = GameManager.taskManager.currentChore;
        if(currentChore.choreTask == TaskManager.Task.LightFireplace)
        {
            foreach(Pickupable pu in l_pu_pickupables)
            {
                if (pu.bl_lighter)
                {
                    img_itemIndicator.sprite = a_spt_itemSprites[0];
                    img_itemIndicator.transform.parent.gameObject.SetActive(true);
                    return;
                }
            }
        }else if (currentChore.choreTask == TaskManager.Task.CleanMirror || currentChore.choreTask == TaskManager.Task.CleanCobwebs)
        {
            foreach (Pickupable pu in l_pu_pickupables)
            {
                if (pu.bl_duster)
                {
                    img_itemIndicator.sprite = a_spt_itemSprites[1];
                    img_itemIndicator.transform.parent.gameObject.SetActive(true);
                    Debug.Log(gameObject.name);
                    return;
                }
            }
        }
        else if (currentChore.choreTask == TaskManager.Task.MopFloor)
        {
            foreach (Pickupable pu in l_pu_pickupables)
            {
                if (pu.bl_mop)
                {
                    img_itemIndicator.sprite = a_spt_itemSprites[2];
                    img_itemIndicator.transform.parent.gameObject.SetActive(true);
                    return;
                }
            }
        }
        else if (currentChore.choreTask == TaskManager.Task.FindKey)
        {
            foreach (Pickupable pu in l_pu_pickupables)
            {
                if (pu.bl_frontDoorKey)
                {
                    img_itemIndicator.sprite = a_spt_itemSprites[3];
                    img_itemIndicator.transform.parent.gameObject.SetActive(true);
                    return;
                }
            }
        }

        img_itemIndicator.transform.parent.gameObject.SetActive(false);

    }

}
