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
    public bool bl_debug;
    public RegionTrigger rt_closet;
    public bool bl_isCloset;
    [Tooltip("Regions that can easily be seen into from the current region.")]
    public RegionTrigger[] a_rt_fullViewRegions;
    RegionTrigger[] a_rt_allOtherRegions;

    private void Start()
    {
        nav_obstacle = GetComponent<NavMeshObstacle>();
        gb_ghost = GameManager.ghost;
        pc_player = GameManager.playerController;
        a_mirrorMovements = FindObjectsOfType<MirrorMovement>();
        a_rt_allOtherRegions = FindObjectsOfType<RegionTrigger>();
        CheckObjects();
        //img_itemIndicator = transform.GetChild(0).GetChild(0).GetComponent<Image>();
    }

    //Checks objects every frame
    private void Update()
    {
        if (bl_debug) Debug.Log(ListToString(l_pu_pickupables));
        if(!bl_isCloset) CheckObjects();
    }

    //Used for debug
    private string ListToString(List<Pickupable> list)
    {
        string str = "";
        foreach(Pickupable pu in list)
        {
            str += pu.name + ", ";
        }
        return str;
    }

    //Assigns current region to characters and objects who enter the region
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pc_player.go_curRegion = gameObject;

            //Activates obstacles if appropriate.(ghost is hiding an object and the ghost is trying to stay hidden)
            if (gb_ghost.bl_hiding && gb_ghost.int_curAggressionLevel < 3)
            {
                BlockGhostPath();
            }

            //Turns on a mirror if appropriate
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
            foreach (RegionTrigger rt in a_rt_allOtherRegions)
            {
                if (rt.l_pu_pickupables.Contains(pu_other) && rt != this)
                {
                    rt.l_pu_pickupables.Remove(pu_other);
                }
            }
        }
    }

    //Handles characters and objects leaving the region
    private void OnTriggerExit(Collider other)
    {
        //Turns off the region's obstacle if needed when player leaves
        if (other.CompareTag("Player") && nav_obstacle.enabled)
        {
            PlayerController pc = GameManager.playerController;
            //Only turns off region's obstacle if the region isn't in full view of the player's current region.
            if (!pc.go_curRegion.GetComponent<RegionTrigger>().a_rt_fullViewRegions.Contains(this))
            {
                nav_obstacle.enabled = false;
            }
            foreach (RegionTrigger rt in a_rt_fullViewRegions)
            {
                if (!pc.go_curRegion.GetComponent<RegionTrigger>().a_rt_fullViewRegions.Contains(rt) && rt != pc.go_curRegion.GetComponent<RegionTrigger>())
                {
                    rt.nav_obstacle.enabled = false;
                }
            }
        }
        else if (other.GetComponent<Pickupable>() != null)
        {
            //Removes pickupable object from list
            Pickupable pu_other = other.GetComponent<Pickupable>();
            l_pu_pickupables.Remove(pu_other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Pickupable pu_other = other.GetComponent<Pickupable>();
        if (pu_other == null) return;
        if (l_pu_pickupables.Contains(pu_other)) return;
        l_pu_pickupables.Add(pu_other);
        foreach (RegionTrigger rt in a_rt_allOtherRegions)
        {
            if (rt.l_pu_pickupables.Contains(pu_other) && rt != this)
            {
                rt.l_pu_pickupables.Remove(pu_other);
            }
        }
    }

    //Turns on all necessary nav obstacles
    public void BlockGhostPath()
    {
        //Makes the ghost drop the item if it would be spotted
        if (gameObject == gb_ghost.go_curRegion || a_rt_fullViewRegions.Contains(gb_ghost.go_curRegion.GetComponent<RegionTrigger>()))
        {
            gb_ghost.GetRobbed();
            return;
        }

        //Makes appropriate obstacles activate
        nav_obstacle.enabled = true;
        foreach (RegionTrigger rt in a_rt_fullViewRegions)
        {
            rt.nav_obstacle.enabled = true;
        }
    }

    //Turns off all necessary nav obstacles
    public void OpenGhostPath()
    {
        nav_obstacle.enabled = false;
        foreach (RegionTrigger rt in a_rt_fullViewRegions)
        {
            rt.nav_obstacle.enabled = false;
        }
    }

    //Handles displaying quest item on the map
    public void CheckObjects()
    {
        Chore currentChore = GameManager.taskManager.currentChore;
        if(currentChore.choreTask == TaskManager.Task.LightFireplace)   //Handles Lighter
        {
            foreach(Pickupable pu in l_pu_pickupables)
            {
                if (pu.bl_lighter)
                {
                    //Displays a lighter on the map in the region if it's present
                    img_itemIndicator.sprite = a_spt_itemSprites[0];
                    img_itemIndicator.transform.parent.gameObject.SetActive(true);
                    return;
                }
            }
        }else if (currentChore.choreTask == TaskManager.Task.CleanCobwebs)    //Handles Duster
        {
            foreach (Pickupable pu in l_pu_pickupables)
            {
                if (pu.bl_duster)
                {
                    //Displays a duster on the map in the region if it's present
                    img_itemIndicator.sprite = a_spt_itemSprites[1];
                    img_itemIndicator.transform.parent.gameObject.SetActive(true);
                    Debug.Log(gameObject.name);
                    return;
                }
            }
        }
        else if (currentChore.choreTask == TaskManager.Task.MopFloor)   //Handles Broom
        {
            foreach (Pickupable pu in l_pu_pickupables)
            {
                if (pu.bl_mop)
                {
                    //Displays a broom on the map in the region if it's present
                    img_itemIndicator.sprite = a_spt_itemSprites[2];
                    img_itemIndicator.transform.parent.gameObject.SetActive(true);
                    return;
                }
            }
        }
        else if (currentChore.choreTask == TaskManager.Task.FindKey)    //Handles Key
        {
            foreach (Pickupable pu in l_pu_pickupables)
            {
                if (pu.bl_frontDoorKey)
                {
                    //Displays a key on the map in the region if it's present
                    img_itemIndicator.sprite = a_spt_itemSprites[3];
                    img_itemIndicator.transform.parent.gameObject.SetActive(true);
                    return;
                }
            }
        }
        if (rt_closet != null)  //Checks the region's closet
        {
            rt_closet.CheckObjects();
            return;
        }
        //Turns off the room's item icon if nothing else in this method was true
        img_itemIndicator.transform.parent.gameObject.SetActive(false);
    }

}
