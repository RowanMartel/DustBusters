using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostBehavior : MonoBehaviour
{
    //Variables that revolve around the Ghost's Navigation Abilities
    [Header("Navigation")]
    public NavMeshAgent agent;
    //The ghost's task list that determines where it travels.
    public List<TaskManager.Task> currentTasks = new List<TaskManager.Task>();  //Current tasks
    public List<pointList> currentPoints = new List<pointList>();   //Current travel points
    public List<TaskManager.Task> startTasks;   //Tasks it has at the beginning of the game
    public List<TaskManager.Task> endGameTasks; //Tasks it gains at the end of the game
    [Tooltip("Every task the ghost can interact with")]
    public List<TaskManager.Task> masterTaskList;   //Every task the ghost can have
    [Tooltip("All patrol points that the ghost can go to for the task that shares it's index in the master task list")]
    public List<pointList> patrolPointsPerTask; //Patrol points matching the tasks from the task list
    public Transform currentPatrolPoint;
    public int curIndex;

    public float distToSwitch;  //Distance between ghost and patrol point at the time of the switch

    //Variables that revolve around attacking the player
    [Header("Attack")]
    public GameObject playerObject; //The player's GameObject
    public float timeToThrow;
    public float curTime;
    public List<GameObject> throwables; //List of objects the ghost is currently floating
    public float attackThrowForce;

    [Header("Aiming")]
    public float sightRange;

    //Variables that revolve around holding an object
    [Header("Holding Items")]
    public GameObject curHeldItem;
    public GameObject heldItemParent;
    public Transform heldItemSpinner;
    public float spinSpeed;

    //Variables around interacting with items
    [Header("Task Item Interaction")]
    bool hiding;
    public Transform[] hidingPlaces;
    public float breakThrowForce;

    //Variables around interacting with Light
    [Header("Light Interaction")]
    public float baseSpeed;
    public float slowedSpeed;
    public List<GameObject> lightSourcesEffecting;

    //Audio Variables
    [Header("Spooky SFX")]
    public AudioClip[] sounds;
    AudioClip lastPlayed;
    public AudioSource aSource;
    public float sfxTime;
    public float sfxTimeDeviationRange;
    public float curSFXTime;

    //Light Switch Variables
    [Header("Light Switch")]
    private LightSwitch[] switches;
    public float lightSwitchDist;


    // Start is called before the first frame update
    void Start()
    {
        //Set Current Tasks to be the starting tasks
        for (int i = 0; i < startTasks.Count; i++)
        {
            currentTasks.Add(startTasks[i]);
            pointList pList = new pointList();
            foreach (Transform item in patrolPointsPerTask[masterTaskList.IndexOf(startTasks[i])].list)
            {
                pList.Add(item);
            }
            currentPoints.Add(pList);
        }

        //Set all to default
        SwitchToPoint(0);
        curTime = timeToThrow;
        curSFXTime = sfxTime;
        hiding = false;
        switches = FindObjectsByType<LightSwitch>(FindObjectsSortMode.InstanceID);
    }

    // Update is called once per frame
    void Update()
    {
        //Check if near lightswitch and turn them off if needed
        LightSwitchCheck();

        //Light Interaction
        if (lightSourcesEffecting.Count > 0)
        {
            agent.speed = slowedSpeed;
        }
        else
        {
            agent.speed = baseSpeed;
        }

        //Move the held item
        heldItemSpinner.Rotate(Vector3.up * spinSpeed);
        if(curHeldItem != null)
        {
            curHeldItem.transform.position = heldItemParent.transform.position;
        }

        //Travel to current patrol point
        agent.SetDestination(currentPatrolPoint.position);
        if(distToSwitch > Vector3.Distance(transform.position, currentPatrolPoint.position))
        {
            //Attempt to interact with patrol point
            Pickupable pickup = currentPatrolPoint.GetComponent<Pickupable>();
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
                    pickup.GetComponent<Rigidbody>().AddForce(pickup.transform.forward * breakThrowForce, ForceMode.Impulse);

                    curIndex++;
                    if (curIndex >= currentPoints.Count)
                    {
                        curIndex = 0;
                    }

                }
            }
            else
            {
                //Start next patrol point
                if (hiding)
                {
                    PlaceItem(currentPatrolPoint.position);
                    hiding = false;
                }
                else if (curHeldItem != null)
                    ChooseHidingPlace();
                else
                {
                    curIndex++;
                    if (curIndex >= currentPoints.Count)
                    {
                        curIndex = 0;
                    }
                    SwitchToPoint(curIndex);
                }
            }
        }

        if (currentTasks.Contains(TaskManager.Task.EscapeHouse))
        {
            //Attack player if player is visible
            RaycastHit hit;
            if (Physics.Raycast(transform.position, playerObject.transform.position - transform.position, out hit, sightRange))
            {
                if (hit.collider.gameObject == playerObject)
                {
                    if (throwables.Count > 0)
                    {
                        curTime -= Time.deltaTime;
                        if (curTime <= 0)
                        {
                            curTime = timeToThrow;
                            GameObject toThrow = throwables[0];
                            toThrow.transform.LookAt(playerObject.transform.position);
                            toThrow.GetComponent<Rigidbody>().AddForce(toThrow.transform.forward * attackThrowForce, ForceMode.Impulse);
                        }
                    }
                }
            }
        }

        //Play Audio
        curSFXTime -= Time.deltaTime;
        if(curSFXTime <= 0)
        {
            PlaySound();
        }
    }

    //Check if Lightswitch is nearby. Turn off if needed
    private void LightSwitchCheck()
    {
        foreach (LightSwitch lightSwitch in switches)
        {
            if(Vector3.Distance(lightSwitch.transform.position, transform.position) <= lightSwitchDist)
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
        if (index < currentPoints.Count)
        {
            int pointIndex = Random.Range(0, currentPoints[index].list.Count);
            currentPatrolPoint = currentPoints[index].list[pointIndex];
            agent.SetDestination(currentPatrolPoint.position);
            curIndex = index;
        }
    }

    //Remove task from ghost's current task list
    public void RemoveTask(TaskManager.Task task)
    {
        if (!currentTasks.Contains(task)){
            Debug.Log("Task Not In Ghost List");
            return;
        }

        int index = currentTasks.IndexOf(task);

        currentTasks.Remove(task);
        currentPoints.Remove(currentPoints[index]);

        if (curIndex == index)
        {
            if (curIndex >= currentPoints.Count)
            {
                curIndex = 0;
            }
            SwitchToPoint(curIndex);
        }

    }

    //Play a random audio clip
    public void PlaySound()
    {
        AudioClip clip;
        do
        {
            clip = sounds[Random.Range(0, sounds.Length - 1)];
        } while (clip == lastPlayed);

        aSource.clip = clip;
        aSource.Play();
        lastPlayed = clip;
        curSFXTime = sfxTime + Random.Range(-sfxTimeDeviationRange, sfxTimeDeviationRange);
    }

    //Add task to current task list
    public void AddTask(TaskManager.Task task)
    {
        currentTasks.Add(task);
        currentPoints.Add(new pointList());
        foreach (Transform item in patrolPointsPerTask[masterTaskList.IndexOf(task)].list)
        {
            currentPoints[currentPoints.Count - 1].Add(item);
        }
    }
    
    //Set current tasks to be the End Game Tasks
    public void EnterEndGame()
    {
        currentTasks.Clear();
        currentPoints.Clear();

        for (int i = 0; i < endGameTasks.Count; i++)
        {
            currentTasks.Add(endGameTasks[i]);
            currentPoints.Add(new pointList());
            foreach (Transform item in patrolPointsPerTask[masterTaskList.IndexOf(endGameTasks[i])].list)
            {
                currentPoints[i].Add(item);
            }
        }

        curIndex = 0;
    }

    //Set an item to be the current held item
    public void PickUpItem(GameObject item)
    {
        if (GameManager.playerController.heldObject != null && GameManager.playerController.heldObject.name == item.name)
        {
            curIndex++;
            if (curIndex >= currentPoints.Count)
            {
                curIndex = 0;
            }
            SwitchToPoint(curIndex);
            return;
        }

        if (curHeldItem != null)
            DropItem();

        item.transform.parent = heldItemParent.transform;
        item.transform.localPosition = Vector3.zero;
        curHeldItem = item;
        Debug.Log("holding " + item.name);
    }

    //Remove current held item from ghost
    public void DropItem()
    {
        curHeldItem.transform.parent = null;
        curHeldItem.transform.position = heldItemParent.transform.position;
        curHeldItem.GetComponent<Rigidbody>().velocity = Vector3.zero;
        curHeldItem.GetComponent<Rigidbody>().Sleep();
        curHeldItem = null;
    }

    //Place current held item in designated location
    void PlaceItem(Vector3 pos)
    {
        GameObject item = curHeldItem;
        DropItem();
        item.transform.position = pos;
        item.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    //set current destination to be a random hiding place
    public void ChooseHidingPlace()
    {
        if (curHeldItem == null) return;
        hiding = true;
        int rand = Random.Range(0, hidingPlaces.Length);
        agent.SetDestination(hidingPlaces[rand].position);
        currentPatrolPoint = hidingPlaces[rand];
    }

    //Add light to light sources list
    public void EnterLight(GameObject lightsource)
    {
        lightSourcesEffecting.Add(lightsource);
    }

    //Remove light from light sources list
    public void ExitLight(GameObject lightsource)
    {
        lightSourcesEffecting.Remove(lightsource);
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
