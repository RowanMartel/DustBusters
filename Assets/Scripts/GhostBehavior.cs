using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostBehavior : MonoBehaviour
{
    [Header("Navigation")]
    public NavMeshAgent agent;

    public List<TaskManager.Task> currentTasks = new List<TaskManager.Task>();
    public List<pointList> currentPoints = new List<pointList>();
    public List<TaskManager.Task> startTasks;
    public List<TaskManager.Task> endGameTasks;
    [Tooltip("Every task the ghost can interact with")]
    public List<TaskManager.Task> masterTaskList;
    [Tooltip("All patrol points that the ghost can go to for the task that shares it's index in the master task list")]
    public List<pointList> patrolPointsPerTask;
    public Transform currentPatrolPoint;
    public int curIndex;

    public float distToSwitch;

    [Header("Attack")]
    public GameObject playerObject;
    public float timeToThrow;
    public float curTime;
    public List<GameObject> throwables;
    public float attackThrowForce;

    [Header("Aiming")]
    public float sightRange;

    [Header("Holding Items")]
    public GameObject curHeldItem;
    public GameObject heldItemParent;
    public Transform heldItemSpinner;
    public float spinSpeed;

    [Header("Task Item Interaction")]
    bool hiding;
    public Transform[] hidingPlaces;
    public float breakThrowForce;

    [Header("Light Interaction")]
    public float baseSpeed;
    public float slowedSpeed;
    public List<GameObject> lightSourcesEffecting;

    [Header("Spooky SFX")]
    public AudioClip[] sounds;
    AudioClip lastPlayed;
    public AudioSource aSource;
    public float sfxTime;
    public float sfxTimeDeviationRange;
    public float curSFXTime;

    // Start is called before the first frame update
    void Start()
    {
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

        SwitchToPoint(0);
        curTime = timeToThrow;
        curSFXTime = sfxTime;
        hiding = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (lightSourcesEffecting.Count > 0)
        {
            agent.speed = slowedSpeed;
        }
        else
        {
            agent.speed = baseSpeed;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            EnterEndGame();
        }

        heldItemSpinner.Rotate(Vector3.up * spinSpeed);

        if(curHeldItem != null)
        {
            curHeldItem.transform.position = heldItemParent.transform.position;
        }

        agent.SetDestination(currentPatrolPoint.position);
        if(distToSwitch > Vector3.Distance(transform.position, currentPatrolPoint.position))
        {
            try
            {
                Pickupable item = currentPatrolPoint.GetComponent<Pickupable>();
                if (item.hideable)
                {
                    PickUpItem(item.gameObject);
                    ChooseHidingPlace();
                }else if (item.breakable)
                {
                    item.transform.LookAt(transform.position);
                    item.GetComponent<Rigidbody>().AddForce(item.transform.forward * breakThrowForce, ForceMode.Impulse);

                    curIndex++;
                    if (curIndex >= currentPoints.Count)
                    {
                        curIndex = 0;
                    }
                    SwitchToPoint(curIndex);
                }
            }
            catch
            {
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

        curSFXTime -= Time.deltaTime;
        if(curSFXTime <= 0)
        {
            PlaySound();
        }
    }

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

    public void AddTask(TaskManager.Task task)
    {
        currentTasks.Add(task);
        currentPoints.Add(new pointList());
        foreach (Transform item in patrolPointsPerTask[masterTaskList.IndexOf(task)].list)
        {
            currentPoints[currentPoints.Count - 1].Add(item);
        }
    }
    
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

    public void PickUpItem(GameObject item)
    {
        if (GameManager.playerController.heldObject.name == item.name)
        {
            curIndex++;
            if (curIndex >= currentPoints.Count)
            {
                curIndex = 0;
            }
            SwitchToPoint(curIndex);
            return;
        }

        if(curHeldItem != null)
            DropItem();

        item.transform.parent = heldItemParent.transform;
        item.transform.localPosition = Vector3.zero;
        curHeldItem = item;
    }

    public void DropItem()
    {
        curHeldItem.transform.parent = null;
        curHeldItem.transform.position = heldItemParent.transform.position;
        curHeldItem.GetComponent<Rigidbody>().velocity = Vector3.zero;
        curHeldItem.GetComponent<Rigidbody>().Sleep();
        curHeldItem = null;
    }

    void PlaceItem(Vector3 pos)
    {
        GameObject item = curHeldItem;
        DropItem();
        item.transform.position = pos;
        item.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void ChooseHidingPlace()
    {
        if (curHeldItem == null) return;
        hiding = true;
        int rand = Random.Range(0, hidingPlaces.Length);
        agent.SetDestination(hidingPlaces[rand].position);
        currentPatrolPoint = hidingPlaces[rand];
    }

    public void EnterLight(GameObject lightsource)
    {
        lightSourcesEffecting.Add(lightsource);
    }

    public void ExitLight(GameObject lightsource)
    {
        lightSourcesEffecting.Remove(lightsource);
    }

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
