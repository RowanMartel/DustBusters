using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class GhostBehavior : MonoBehaviour
{
    //Variables that revolve around the Ghost's Navigation Abilities
    [Header("Navigation")]
    public NavMeshAgent nav_agent;
    //The ghost's task list that determines where it travels.
    public List<TaskManager.Task> l_tsk_currentTasks = new List<TaskManager.Task>();  //Current tasks
    public List<pointList> l_pl_currentPoints = new List<pointList>();   //Current travel points
    public List<TaskManager.Task> l_tsk_startTasks;   //Tasks it has at the beginning of the game
    public List<TaskManager.Task> l_tsk_endGameTasks; //Tasks it gains at the end of the game
    [Tooltip("Every task the ghost can interact with")]
    public List<TaskManager.Task> l_tsk_masterTaskList;   //Every task the ghost can have
    [Tooltip("All patrol points that the ghost can go to for the task that shares it's index in the master task list")]
    public List<pointList> l_pl_patrolPointsPerTask; //Patrol points matching the tasks from the task list
    public Transform tr_currentPatrolPoint;
    public int int_curIndex;

    public float flt_distToSwitch;  //Distance between ghost and patrol point at the time of the switch

    //Variables that revolve around attacking the player
    [Header("Attack")]
    public GameObject go_player; //The player's GameObject
    public float flt_timeToThrow;
    public float flt_curTime;
    public List<GameObject> l_go_throwables; //List of objects the ghost is currently floating
    public float flt_attackThrowForce;

    [Header("Aiming")]
    public float flt_sightRange;

    //Variables that revolve around holding an object
    [Header("Holding Items")]
    public GameObject go_curHeldItem;
    public GameObject go_heldItemParent;
    public Transform tr_heldItemSpinner;
    public float flt_spinSpeed;

    //Variables around interacting with items
    [Header("Task Item Interaction")]
    bool bl_hiding;
    public Transform[] a_tr_hidingPlaces;
    public float flt_breakThrowForce;

    //Variables around interacting with Light
    [Header("Light Interaction")]
    public float flt_baseSpeed;
    public float flt_slowedSpeed;
    public List<GameObject> l_go_lightSourcesEffecting;

    //Audio Variables
    [Header("Spooky SFX")]
    public AudioClip[] a_ac_sounds;
    AudioClip ac_lastPlayed;
    public AudioSource as_aSource;
    public float flt_sfxTime;
    public float flt_sfxTimeDeviationRange;
    public float flt_curSFXTime;

    //Light Switch Variables
    [Header("Light Switch")]
    private LightSwitch[] a_ls_switches;
    public float flt_lightSwitchDist;


    // Start is called before the first frame update
    void Start()
    {
        //Set Current Tasks to be the starting tasks
        for (int i = 0; i < l_tsk_startTasks.Count; i++)
        {
            l_tsk_currentTasks.Add(l_tsk_startTasks[i]);
            pointList pl_pList = new pointList();
            foreach (Transform item in l_pl_patrolPointsPerTask[l_tsk_masterTaskList.IndexOf(l_tsk_startTasks[i])].list)
            {
                pl_pList.Add(item);
            }
            l_pl_currentPoints.Add(pl_pList);
        }

        //Set all to default
        SwitchToPoint(0);
        flt_curTime = flt_timeToThrow;
        flt_curSFXTime = flt_sfxTime;
        bl_hiding = false;
        a_ls_switches = FindObjectsByType<LightSwitch>(FindObjectsSortMode.InstanceID);
    }

    // Update is called once per frame
    void Update()
    {
        //Check if near lightswitch and turn them off if needed
        LightSwitchCheck();

        //Test Material To Remove Later
        if (Input.GetKeyDown(KeyCode.R))
        {
            EnterEndGame();
        }

        //Light Interaction
        if (l_go_lightSourcesEffecting.Count > 0)
        {
            nav_agent.speed = flt_slowedSpeed;
        }
        else
        {
            nav_agent.speed = flt_baseSpeed;
        }

        //Move the held item
        tr_heldItemSpinner.Rotate(Vector3.up * flt_spinSpeed);
        if(go_curHeldItem != null)
        {
            go_curHeldItem.transform.position = go_heldItemParent.transform.position;
        }

        //Travel to current patrol point
        nav_agent.SetDestination(tr_currentPatrolPoint.position);
        if(flt_distToSwitch > Vector3.Distance(transform.position, tr_currentPatrolPoint.position))
        {
            //Attempt to interact with patrol point
            Pickupable pickup = tr_currentPatrolPoint.GetComponent<Pickupable>();
            if (pickup != null)
            {
                if (pickup.hideable)
                {
                    PickUpItem(pickup.gameObject);
                    ChooseHidingPlace();
                }
                else if (pickup.breakable)
                {
                    pickup.transform.LookAt(transform.position);
                    pickup.GetComponent<Rigidbody>().AddForce(pickup.transform.forward * flt_breakThrowForce, ForceMode.Impulse);

                    int_curIndex++;
                    if (int_curIndex >= l_pl_currentPoints.Count)
                    {
                        int_curIndex = 0;
                    }

                }
            }
            else
            {
                //Start next patrol point
                if (bl_hiding)
                {
                    PlaceItem(tr_currentPatrolPoint.position);
                    bl_hiding = false;
                }
                else if (go_curHeldItem != null)
                    ChooseHidingPlace();
                else
                {
                    int_curIndex++;
                    if (int_curIndex >= l_pl_currentPoints.Count)
                    {
                        int_curIndex = 0;
                    }
                    SwitchToPoint(int_curIndex);
                }
            }
        }

        if (l_tsk_currentTasks.Contains(TaskManager.Task.EscapeHouse))
        {
            //Attack player if player is visible
            RaycastHit hit;
            if (Physics.Raycast(transform.position, go_player.transform.position - transform.position, out hit, flt_sightRange))
            {
                if (hit.collider.gameObject == go_player)
                {
                    if (l_go_throwables.Count > 0)
                    {
                        flt_curTime -= Time.deltaTime;
                        if (flt_curTime <= 0)
                        {
                            flt_curTime = flt_timeToThrow;
                            GameObject go_toThrow = l_go_throwables[0];

                            foreach (GameObject go_throwable in l_go_throwables)
                            {
                                Pickupable pu_throwable = go_throwable.GetComponent<Pickupable>();
                                if(pu_throwable != null)
                                {
                                    if (pu_throwable.canDamagePlayer)
                                    {
                                        go_toThrow = go_throwable;
                                    }
                                }
                            }

                            go_toThrow.transform.LookAt(go_player.transform.position);
                            go_toThrow.GetComponent<Rigidbody>().AddForce(go_toThrow.transform.forward * flt_attackThrowForce, ForceMode.Impulse);
                        }
                    }
                }
            }
        }

        //Play Audio
        flt_curSFXTime -= Time.deltaTime;
        if(flt_curSFXTime <= 0)
        {
            PlaySound();
        }
    }

    //Check if Lightswitch is nearby. Turn off if needed
    private void LightSwitchCheck()
    {
        foreach (LightSwitch lightSwitch in a_ls_switches)
        {
            if(Vector3.Distance(lightSwitch.transform.position, transform.position) <= flt_lightSwitchDist)
            {
                if (lightSwitch.on)
                {
                    lightSwitch.Interact();
                }
            }
        }
    }

    //Set ghost's destination to the appropriate point
    void SwitchToPoint(int index)
    {
        if (index < l_pl_currentPoints.Count)
        {
            int int_pointIndex = Random.Range(0, l_pl_currentPoints[index].list.Count);
            tr_currentPatrolPoint = l_pl_currentPoints[index].list[int_pointIndex];
            nav_agent.SetDestination(tr_currentPatrolPoint.position);
            int_curIndex = index;
        }
    }

    //Remove task from ghost's current task list
    public void RemoveTask(TaskManager.Task tsk_task)
    {
        if (!l_tsk_currentTasks.Contains(tsk_task)){
            Debug.Log("Task Not In Ghost List");
            return;
        }

        int int_index = l_tsk_currentTasks.IndexOf(tsk_task);

        l_tsk_currentTasks.Remove(tsk_task);
        l_pl_currentPoints.Remove(l_pl_currentPoints[int_index]);

        if (int_curIndex == int_index)
        {
            if (int_curIndex >= l_pl_currentPoints.Count)
            {
                int_curIndex = 0;
            }
            SwitchToPoint(int_curIndex);
        }

    }

    //Play a random audio clip
    public void PlaySound()
    {
        AudioClip ac_clip;
        do
        {
            ac_clip = a_ac_sounds[Random.Range(0, a_ac_sounds.Length - 1)];
        } while (ac_clip == ac_lastPlayed);

        as_aSource.clip = ac_clip;
        as_aSource.Play();
        ac_lastPlayed = ac_clip;
        flt_curSFXTime = flt_sfxTime + Random.Range(-flt_sfxTimeDeviationRange, flt_sfxTimeDeviationRange);
    }

    //Add task to current task list
    public void AddTask(TaskManager.Task tsk_task)
    {
        l_tsk_currentTasks.Add(tsk_task);
        l_pl_currentPoints.Add(new pointList());
        foreach (Transform tr_item in l_pl_patrolPointsPerTask[l_tsk_masterTaskList.IndexOf(tsk_task)].list)
        {
            l_pl_currentPoints[l_pl_currentPoints.Count - 1].Add(tr_item);
        }
    }
    
    //Set current tasks to be the End Game Tasks
    public void EnterEndGame()
    {
        l_tsk_currentTasks.Clear();
        l_pl_currentPoints.Clear();

        for (int i = 0; i < l_tsk_endGameTasks.Count; i++)
        {
            l_tsk_currentTasks.Add(l_tsk_endGameTasks[i]);
            l_pl_currentPoints.Add(new pointList());
            foreach (Transform item in l_pl_patrolPointsPerTask[l_tsk_masterTaskList.IndexOf(l_tsk_endGameTasks[i])].list)
            {
                l_pl_currentPoints[i].Add(item);
            }
        }

        int_curIndex = 0;
    }

    //Set an item to be the current held item
    public void PickUpItem(GameObject go_item)
    {
        if (GameManager.playerController.heldObject != null && GameManager.playerController.heldObject.name == go_item.name)
        {
            int_curIndex++;
            if (int_curIndex >= l_pl_currentPoints.Count)
            {
                int_curIndex = 0;
            }
            SwitchToPoint(int_curIndex);
            return;
        }

        if (go_curHeldItem != null)
            DropItem();

        go_item.transform.parent = go_heldItemParent.transform;
        go_item.transform.localPosition = Vector3.zero;
        go_curHeldItem = go_item;
        Debug.Log("holding " + go_item.name);
    }

    //Remove current held item from ghost
    public void DropItem()
    {
        go_curHeldItem.transform.parent = null;
        go_curHeldItem.transform.position = go_heldItemParent.transform.position;
        go_curHeldItem.GetComponent<Rigidbody>().velocity = Vector3.zero;
        go_curHeldItem.GetComponent<Rigidbody>().Sleep();
        go_curHeldItem = null;
    }

    //Place current held item in designated location
    void PlaceItem(Vector3 pos)
    {
        GameObject go_item = go_curHeldItem;
        DropItem();
        go_item.transform.position = pos;
        go_item.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    //set current destination to be a random hiding place
    public void ChooseHidingPlace()
    {
        if (go_curHeldItem == null) return;
        bl_hiding = true;
        int rand = Random.Range(0, a_tr_hidingPlaces.Length);
        nav_agent.SetDestination(a_tr_hidingPlaces[rand].position);
        tr_currentPatrolPoint = a_tr_hidingPlaces[rand];
    }

    //Add light to light sources list
    public void EnterLight(GameObject lightsource)
    {
        l_go_lightSourcesEffecting.Add(lightsource);
    }

    //Remove light from light sources list
    public void ExitLight(GameObject lightsource)
    {
        l_go_lightSourcesEffecting.Remove(lightsource);
    }

    //Allows a list of lists to be filled in the inspector
    [System.Serializable]
    public class pointList
    {
        public List<Transform> list;

        public pointList()
        {
            list = new List<Transform>();
        }

        public void Add(Transform item)
        {
            list.Add(item);
        }

    }

}
