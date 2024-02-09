using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class GhostBehavior : MonoBehaviour
{

    PlayerController pc_player;

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
    public GameObject go_curRegion;

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
    public bool bl_hiding;
    public HidingSpot[] a_hs_hidingPlaces;
    HidingSpot hs_curHidingSpot;
    public float flt_breakThrowForce;
    public float flt_douseFireplaceChance;
    public float flt_dirtyMirrorChance;
    public float flt_dirtyFloorChance;
    public float flt_throwBookChance;
    public float flt_throwDishChance;

    //Variables around interacting with Light
    [Header("Light Interaction")]
    public float flt_baseSpeed;
    public float flt_aggroSpeed = 6;
    public float flt_slowedSpeed;
    public float flt_aggroSlowedSpeed = 3;
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
    /*public*/ float flt_lightSwitchCooldown = 5;
    public float flt_curSwitchCooldown;


    //Aggression Levels
    [Header("Aggression Levels")]
    public int int_curAggressionLevel;
    public int int_tasksToStage2;
    public int int_tasksToStage3;
    public List<TaskManager.Task> l_tsk_completedTasks;
    public GameObject go_floatTrigger;

    public bool bl_frozen;


    // Start is called before the first frame update
    void Start()
    {
        pc_player = GameManager.playerController;

        //Set Current Tasks to be the starting tasks
        for (int i = 0; i < l_tsk_startTasks.Count; i++)
        {
            l_tsk_currentTasks.Add(l_tsk_startTasks[i]);
            pointList pl_pList = new pointList();
            foreach (Transform item in l_pl_patrolPointsPerTask[l_tsk_masterTaskList.IndexOf(l_tsk_startTasks[i])].listAggro1)
            {
                pl_pList.Add1(item);
            }
            foreach (Transform item in l_pl_patrolPointsPerTask[l_tsk_masterTaskList.IndexOf(l_tsk_startTasks[i])].listAggro2)
            {
                pl_pList.Add2(item);
            }
            foreach (Transform item in l_pl_patrolPointsPerTask[l_tsk_masterTaskList.IndexOf(l_tsk_startTasks[i])].listAggro3)
            {
                pl_pList.Add3(item);
            }
            foreach (Transform item in l_pl_patrolPointsPerTask[l_tsk_masterTaskList.IndexOf(l_tsk_startTasks[i])].listAggro4)
            {
                pl_pList.Add4(item);
            }
            l_pl_currentPoints.Add(pl_pList);
        }

        //Initialize
        SwitchToPoint(0);
        flt_curTime = flt_timeToThrow;
        flt_curSFXTime = flt_sfxTime;
        flt_curSwitchCooldown = flt_lightSwitchCooldown;
        bl_hiding = false;
        a_ls_switches = FindObjectsByType<LightSwitch>(FindObjectsSortMode.InstanceID);
        bl_frozen = false;
        go_floatTrigger.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        //The below happens for all aggression levels

        SetGhostSpeed();

        //Move the held item parent around the ghost.
        if (!bl_frozen)
        {
            tr_heldItemSpinner.Rotate(Vector3.up * flt_spinSpeed);
        }
        if (go_curHeldItem != null)
        {
            //go_curHeldItem.transform.position = go_heldItemParent.transform.position;
        }

        //The below is aggression level dependent
        switch (int_curAggressionLevel)
        {
            case 1:
                
                break;
            case 2:
                //Check if near lightswitch and turn them off if needed
                if (flt_curSwitchCooldown <= 0)
                {
                    LightSwitchCheck();
                }
                else
                {
                    flt_curSwitchCooldown -= Time.deltaTime;
                }

                //Play Audio
                flt_curSFXTime -= Time.deltaTime;
                if (flt_curSFXTime <= 0)
                {
                    GameManager.soundManager.PlayClip(a_ac_sounds, as_aSource);
                    flt_curSFXTime = flt_sfxTime + Random.Range(-flt_sfxTimeDeviationRange, flt_sfxTimeDeviationRange);
                }

                break;
            case 3:

                //Check if near lightswitch and turn them off if needed
                if (flt_curSwitchCooldown <= 0)
                {
                    LightSwitchCheck();
                }
                else
                {
                    flt_curSwitchCooldown -= Time.deltaTime;
                }

                //Attack player if player is visible and done cooldown
                if (CanSeePlayer() && l_go_throwables.Count > 0)
                {
                    flt_curTime -= Time.deltaTime;
                    if (flt_curTime <= 0)
                    {
                        flt_curTime = flt_timeToThrow;

                        //Get player according to aggro
                        GameObject go_toThrow = ChooseObjectToThrow();
                        //Throw object
                        ThrowObjectAt(go_toThrow, go_player.transform.position);
                    }
                }

                //Play Audio
                flt_curSFXTime -= Time.deltaTime;
                if (flt_curSFXTime <= 0)
                {
                    GameManager.soundManager.PlayClip(a_ac_sounds, as_aSource);
                    flt_curSFXTime = flt_sfxTime + Random.Range(-flt_sfxTimeDeviationRange, flt_sfxTimeDeviationRange);
                }

                break;

            case 4:

                //Check if near lightswitch and turn them off if needed
                if (flt_curSwitchCooldown <= 0)
                {
                    LightSwitchCheck();
                }
                else
                {
                    flt_curSwitchCooldown -= Time.deltaTime;
                }

                //Attack player if player is visible and done cooldown
                if (CanSeePlayer() && l_go_throwables.Count > 0)
                {
                    flt_curTime -= Time.deltaTime;
                    if (flt_curTime <= 0)
                    {
                        flt_curTime = flt_timeToThrow;

                        //Get player according to aggro
                        GameObject go_toThrow = ChooseObjectToThrow();

                        //Throw object
                        ThrowObjectAt(go_toThrow, go_player.transform.position);
                    }
                }

                //Play Audio
                flt_curSFXTime -= Time.deltaTime;
                if (flt_curSFXTime <= 0)
                {
                    GameManager.soundManager.PlayClip(a_ac_sounds, as_aSource);
                    flt_curSFXTime = flt_sfxTime + Random.Range(-flt_sfxTimeDeviationRange, flt_sfxTimeDeviationRange);
                }

                break;
        }

        //The below happens during all aggression levels

        //Travel to current patrol point and perform task
        nav_agent.SetDestination(tr_currentPatrolPoint.position);
        if(flt_distToSwitch > Vector3.Distance(transform.position, tr_currentPatrolPoint.position))
        {
            PerformTask();

            if (!bl_hiding)
            {
                int_curIndex++;
                if (int_curIndex >= l_pl_currentPoints.Count)
                {
                    int_curIndex = 0;
                }
                SwitchToPoint(int_curIndex);
            }
        }

        //Switch Hiding Spot if player enters that region
        if (bl_hiding)
        {
            if (hs_curHidingSpot.a_go_region.Contains<GameObject>(pc_player.go_curRegion))
            {
                ChooseHidingPlace();
            }
        }

        
    }

    bool CanSeePlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, go_player.transform.position - transform.position, out hit, flt_sightRange))
        {
            if (hit.collider.gameObject == go_player) return true;
        }
        return false;
    }

    //Set the ghost's speed in accordance with it's aggression level and whether it's in light
    void SetGhostSpeed()
    {
        if (bl_frozen)
        {
            nav_agent.speed = 0;
            return;
        }

        if (int_curAggressionLevel < 4)
        {
            if (l_go_lightSourcesEffecting.Count > 0)
            {
                nav_agent.speed = flt_slowedSpeed;
            }
            else
            {
                nav_agent.speed = flt_baseSpeed;
            }
            return;
        }

        if(int_curAggressionLevel == 4)
        {
            if (l_go_lightSourcesEffecting.Count > 0)
            {
                nav_agent.speed = flt_aggroSlowedSpeed;
            }
            else
            {
                nav_agent.speed = flt_aggroSpeed;
            }
            return;
        }
    }


    void ThrowObjectAt(GameObject go_toThrow, Vector3 v3_pos)
    {
        go_toThrow.transform.LookAt(v3_pos);
        go_toThrow.GetComponent<Rigidbody>().AddForce(go_toThrow.transform.forward * flt_attackThrowForce, ForceMode.Impulse);
    }


    GameObject ChooseObjectToThrow()
    {
        GameObject go_toThrow = l_go_throwables[0];

        foreach (GameObject go_throwable in l_go_throwables)
        {
            //Prioritize damaging or nondamaging objects depending on aggression level
            Pickupable pu_throwable = go_throwable.GetComponent<Pickupable>();
            if (int_curAggressionLevel >= 4)
            {
                if (pu_throwable != null)
                {
                    if (pu_throwable.bl_canDamagePlayer)
                    {
                        go_toThrow = go_throwable;
                    }
                }
            }
            else
            {
                if (pu_throwable != null)
                {
                    if (pu_throwable.bl_canDamagePlayer)
                    {
                        if (!pu_throwable.bl_canDamagePlayer)
                        {
                            go_toThrow = go_throwable;
                        }
                    }
                }
            }
        }
        return go_toThrow;
    }

    void PerformTask()
    {

        if (int_curAggressionLevel < 3 && go_curRegion == pc_player.go_curRegion) return;
        //Attempt to interact with patrol point
        Pickupable pickup = tr_currentPatrolPoint.GetComponent<Pickupable>();
        if (pickup != null)
        {
            if (pickup.bl_hideable)
            {
                PickUpItem(pickup.gameObject);
                ChooseHidingPlace();
                return;
            }
            
            if (pickup.bl_breakable)
            {

                if (pickup.gameObject.GetComponent<Dish>() != null)
                {
                    int int_rand = Random.Range(0, 10);
                    if (int_rand <= flt_throwDishChance)
                    {
                        pickup.transform.LookAt(transform.position);
                        pickup.GetComponent<Rigidbody>().AddForce(pickup.transform.forward * flt_breakThrowForce, ForceMode.Impulse);
                    }
                    return;
                }

                if (pickup.gameObject.GetComponent<Book>() != null)
                {
                    int int_rand = Random.Range(0, 10);
                    if (int_rand <= flt_throwBookChance)
                    {
                        pickup.transform.LookAt(transform.position);
                        pickup.GetComponent<Rigidbody>().AddForce(pickup.transform.forward * flt_breakThrowForce, ForceMode.Impulse);
                    }
                    return;
                }
            }

            return;

        }

        //Attempt to douse fireplace
        Fireplace fireplace = tr_currentPatrolPoint.GetComponent<Fireplace>();
        if (fireplace != null)
        {
            int int_rand = Random.Range(0, 10);
            if (int_rand <= flt_douseFireplaceChance && int_curAggressionLevel >= 2)
            {
                fireplace.UnLight();
            }
            return;
        }

        //Attempt to dirty mirror
        Mirror mr_mirror = tr_currentPatrolPoint.GetComponent<Mirror>();
        if (mr_mirror != null)
        {
            int int_rand = Random.Range(0, 10);
            if (int_rand <= flt_dirtyMirrorChance && int_curAggressionLevel >= 2)
            {
                mr_mirror.GhostDirty(int_curAggressionLevel);
            }
            return;
        }

        //Attempt to dirty floor
        FloorMess fm_mess = tr_currentPatrolPoint.GetComponent<FloorMess>();
        if (fm_mess != null)
        {
            int int_rand = Random.Range(0, 10);
            if (int_rand <= flt_dirtyFloorChance && int_curAggressionLevel >= 2)
            {
                fm_mess.GhostDirty(int_curAggressionLevel);
            }
            return;
        }

        //Attempt to shut off fuse box
        FuseBox fb_fuseBox = tr_currentPatrolPoint.GetComponent<FuseBox>();
        if (fb_fuseBox != null)
        {
            if (fb_fuseBox.bl_isOn)
            {
                fb_fuseBox.Interact();
            }
            return;
        }

        //Attempt to hide item
        HidingSpot hs_spot = tr_currentPatrolPoint.GetComponent<HidingSpot>();
        if(hs_spot != null)
        {
            PlaceItem(hs_spot.transform.position);
            return;
        }
    }

    //Check if Lightswitch is nearby. Turn off if needed
    private void LightSwitchCheck()
    {
        foreach (LightSwitch lightSwitch in a_ls_switches)
        {
            if(Vector3.Distance(lightSwitch.transform.position, transform.position) <= flt_lightSwitchDist)
            {
                if (lightSwitch.bl_on)
                {
                    if (int_curAggressionLevel == 2 && lightSwitch.a_go_regions.Contains<GameObject>(pc_player.go_curRegion) == false)
                    {
                        lightSwitch.Interact();
                        flt_curSwitchCooldown = flt_lightSwitchCooldown;
                    }else if (int_curAggressionLevel >= 3)
                    {
                        lightSwitch.Interact();
                        flt_curSwitchCooldown = flt_lightSwitchCooldown;
                    }
                }
            }
        }
    }

    //Set ghost's destination to the appropriate point
    void SwitchToPoint(int index)
    {
        if (index < l_pl_currentPoints.Count)
        {
            List<Transform> l_tr_points;
            switch (int_curAggressionLevel)
            {
                default:
                case 1:
                    l_tr_points = l_pl_currentPoints[index].listAggro1;
                    break;
                case 2:
                    l_tr_points = l_pl_currentPoints[index].listAggro2;
                    break;
                case 3:
                    l_tr_points = l_pl_currentPoints[index].listAggro3;
                    break;
                case 4:
                    l_tr_points = l_pl_currentPoints[index].listAggro4;
                    break;
            }
            int int_pointIndex = Random.Range(0, l_tr_points.Count);
            tr_currentPatrolPoint = l_tr_points[int_pointIndex];
            nav_agent.SetDestination(tr_currentPatrolPoint.position);
            int_curIndex = index;
        }
    }

    //Remove task from ghost's current task list
    public void RemoveTask(TaskManager.Task tsk_task)
    {
        if (!l_tsk_currentTasks.Contains(tsk_task)){
            return;
        }

        int int_index = l_tsk_currentTasks.IndexOf(tsk_task);

        l_tsk_currentTasks.Remove(tsk_task);
        l_pl_currentPoints.Remove(l_pl_currentPoints[int_index]);

        if(int_index < int_curIndex)
        {
            int_curIndex--;
        }else if(int_curIndex >= l_tsk_currentTasks.Count)
        {
            int_curIndex = 0;
        }
        SwitchToPoint(int_curIndex);

        //Set aggression level based on tasks completed
        if(l_tsk_completedTasks.Contains(tsk_task) == false)
        {
            l_tsk_completedTasks.Add(tsk_task);
            switch (int_curAggressionLevel)
            {
                default:
                case 1:
                    if (l_tsk_completedTasks.Count >= int_tasksToStage2)
                    {
                        SetAggressionLevel(2);
                    }
                    break;
                case 2:
                    if (l_tsk_completedTasks.Count >= int_tasksToStage3)
                    {
                        SetAggressionLevel(3);
                    }
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }

        //Add tasks to undo player tasks
        switch (tsk_task)
        {
            case TaskManager.Task.LightFireplace:
                AddTask(TaskManager.Task.GhostDouseFireplace);
                break;
            case TaskManager.Task.CleanMirror:
                AddTask(TaskManager.Task.GhostDirtyMirror);
                break;
            case TaskManager.Task.MopFloor:
                AddTask(TaskManager.Task.GhostDirtyFloor);
                break;
                
        }

    }

    //Add task to current task list
    public void AddTask(TaskManager.Task tsk_task)
    {
        if (l_tsk_currentTasks.Contains(tsk_task)) return;
        l_tsk_currentTasks.Add(tsk_task);
        l_pl_currentPoints.Add(new pointList());
        foreach (Transform tr_item in l_pl_patrolPointsPerTask[l_tsk_masterTaskList.IndexOf(tsk_task)].listAggro1)
        {
            l_pl_currentPoints[l_pl_currentPoints.Count - 1].Add1(tr_item);
        }
        foreach (Transform tr_item in l_pl_patrolPointsPerTask[l_tsk_masterTaskList.IndexOf(tsk_task)].listAggro2)
        {
            l_pl_currentPoints[l_pl_currentPoints.Count - 1].Add2(tr_item);
        }
        foreach (Transform tr_item in l_pl_patrolPointsPerTask[l_tsk_masterTaskList.IndexOf(tsk_task)].listAggro3)
        {
            l_pl_currentPoints[l_pl_currentPoints.Count - 1].Add3(tr_item);
        }
        foreach (Transform tr_item in l_pl_patrolPointsPerTask[l_tsk_masterTaskList.IndexOf(tsk_task)].listAggro4)
        {
            l_pl_currentPoints[l_pl_currentPoints.Count - 1].Add4(tr_item);
        }
    }

    //Remove patrol point from patrol points per task list
    public void RemovePoint(Transform tr_point)
    {
        foreach(pointList pl_list in l_pl_patrolPointsPerTask)
        {
            if (pl_list.listAggro1.Contains(tr_point))
            {
                pl_list.listAggro1.Remove(tr_point);
            }
            if (pl_list.listAggro2.Contains(tr_point))
            {
                pl_list.listAggro2.Remove(tr_point);
            }
            if (pl_list.listAggro3.Contains(tr_point))
            {
                pl_list.listAggro3.Remove(tr_point);
            }
            if (pl_list.listAggro4.Contains(tr_point))
            {
                pl_list.listAggro4.Remove(tr_point);
            }
        }
    }

    //Set current tasks to be the End Game Tasks
    public void EnterEndGame()
    {
        l_tsk_currentTasks.Clear();
        l_pl_currentPoints.Clear();

        int_curAggressionLevel = 4;

        for (int i = 0; i < l_tsk_endGameTasks.Count; i++)
        {
            l_tsk_currentTasks.Add(l_tsk_endGameTasks[i]);
            l_pl_currentPoints.Add(new pointList());
            foreach (Transform item in l_pl_patrolPointsPerTask[l_tsk_masterTaskList.IndexOf(l_tsk_endGameTasks[i])].listAggro1)
            {
                l_pl_currentPoints[i].Add1(item);
            }
            foreach (Transform item in l_pl_patrolPointsPerTask[l_tsk_masterTaskList.IndexOf(l_tsk_endGameTasks[i])].listAggro2)
            {
                l_pl_currentPoints[i].Add2(item);
            }
            foreach (Transform item in l_pl_patrolPointsPerTask[l_tsk_masterTaskList.IndexOf(l_tsk_endGameTasks[i])].listAggro3)
            {
                l_pl_currentPoints[i].Add3(item);
            }
            foreach (Transform item in l_pl_patrolPointsPerTask[l_tsk_masterTaskList.IndexOf(l_tsk_endGameTasks[i])].listAggro4)
            {
                l_pl_currentPoints[i].Add4(item);
            }
        }

        int_curIndex = 0;
    }

    //Set an item to be the current held item
    public void PickUpItem(GameObject go_item)
    {
        if (go_item.GetComponent<Pickupable>() == null)
        {
            return;
        }

        if (pc_player.Go_heldObject != null && pc_player.Go_heldObject.name == go_item.name)
        {
            int_curIndex++;
            if (int_curIndex >= l_pl_currentPoints.Count)
            {
                int_curIndex = 0;
            }
            SwitchToPoint(int_curIndex);
            return;
        }

        if(go_item.GetComponent<Floatable>() != null)
        {
            l_go_throwables.Remove(go_item);
            go_item.GetComponent<Floatable>().StopFloat();
        }

        if (go_curHeldItem != null)
            DropItem();

        go_item.transform.parent = go_heldItemParent.transform;
        go_item.transform.localPosition = Vector3.zero;
        go_item.GetComponent<Rigidbody>().useGravity = false;
        go_item.GetComponent<Rigidbody>().isKinematic = true;
        go_curHeldItem = go_item;
    }

    //Remove current held item from ghost
    public void DropItem()
    {
        go_curHeldItem.transform.parent = null;
        go_curHeldItem.GetComponent<Rigidbody>().velocity = Vector3.zero;
        go_curHeldItem.GetComponent<Rigidbody>().useGravity = true;
        go_curHeldItem.GetComponent<Rigidbody>().isKinematic = false;
        if (bl_hiding)
        {
            bl_hiding = false;
            pc_player.go_curRegion.GetComponent<NavMeshObstacle>().enabled = false;
        }
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
        if(int_curAggressionLevel < 3)
            pc_player.go_curRegion.GetComponent<NavMeshObstacle>().enabled = true;
        do
        {
            bl_hiding = true;
            int rand = Random.Range(0, a_hs_hidingPlaces.Length);
            hs_curHidingSpot = a_hs_hidingPlaces[rand];
            nav_agent.SetDestination(hs_curHidingSpot.transform.position);
            tr_currentPatrolPoint = hs_curHidingSpot.transform;
        } while (hs_curHidingSpot.a_go_region.Contains<GameObject>(pc_player.go_curRegion));
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

    //Player takes ghost's item
    public void GetRobbed()
    {
        DropItem();
        if (int_curIndex >= l_pl_currentPoints.Count)
        {
            int_curIndex = 0;
        }
        SwitchToPoint(int_curIndex);
    }

    public void SetAggressionLevel(int int_newAggroLevel)
    {
        int_curAggressionLevel = int_newAggroLevel;
        switch(int_curAggressionLevel)
        {
            case 1:
                //Deactivate Float Trigger if required
                if (go_floatTrigger.activeSelf == true)
                {
                    go_floatTrigger.GetComponent<FloatTrigger>().CloseTrigger();
                }
                break;
            case 2:
                //Deactivate Float Trigger if required
                if (go_floatTrigger.activeSelf == true)
                {
                    go_floatTrigger.GetComponent<FloatTrigger>().CloseTrigger();
                }
                break;
            case 3:
                //Activate Float Trigger if required
                if (go_floatTrigger.activeSelf == false)
                {
                    go_floatTrigger.SetActive(true);
                }
                AddTask(TaskManager.Task.ResetBreakerBox);
                break;
            case 4:
                //Activate Float Trigger if required
                if (go_floatTrigger.activeSelf == false)
                {
                    go_floatTrigger.SetActive(true);
                }
                break;
        }

        SwitchToPoint(int_curIndex);
    }

    //Allows a list of lists to be filled in the inspector
    [System.Serializable]
    public class pointList
    {
        public List<Transform> listAggro1;
        public List<Transform> listAggro2;
        public List<Transform> listAggro3;
        public List<Transform> listAggro4;

        public pointList()
        {
            listAggro1 = new List<Transform>();
            listAggro2 = new List<Transform>();
            listAggro3 = new List<Transform>();
            listAggro4 = new List<Transform>();
        }

        public void Add1(Transform item)
        {
            listAggro1.Add(item);
        }
        public void Add2(Transform item)
        {
            listAggro2.Add(item);
        }
        public void Add3(Transform item)
        {
            listAggro3.Add(item);
        }
        public void Add4(Transform item)
        {
            listAggro4.Add(item);
        }

    }
}
