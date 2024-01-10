using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostBehavior : MonoBehaviour
{
    [Header("Navigation")]
    public NavMeshAgent agent;

    //List<Transform> patrolPoints;
    //public List<Transform> startingPoints;
    //public List<Transform> endGamePoints;
    List<TaskManager.Task> currentTasks = new List<TaskManager.Task>();
    List<pointList> currentPoints = new List<pointList>();
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
    public float throwForce;

    [Header("Aiming")]
    public float sightRange;

    [Header("Holding Items")]
    public GameObject curHeldItem;
    public GameObject heldItemParent;
    public Transform heldItemSpinner;
    public float spinSpeed;

    [Header("Hiding Items")]
    bool hiding;
    //public GameObject key;
    public Transform[] hidingPlaces;

    [Header("Light Interaction")]
    public float baseSpeed;
    public float slowedSpeed;
    public List<GameObject> lightSourcesEffecting;

    // Start is called before the first frame update
    void Start()
    {
        /*foreach (TaskManager.Task item in startTasks)
        {
            currentTasks.Add(item);

        }
        foreach (List<Transform> item in startPointsPerTask)
        {
            currentPoints.Add(new List<Transform>());
            foreach (Transform trans in item)
            {
                currentPoints[currentPoints.Count - 1].Add(trans);
            }
        }*/

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
        hiding = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (IsInLight())
        {
            agent.speed = slowedSpeed;
        }
        else
        {
            agent.speed = baseSpeed;
        }

        if (Input.GetKeyDown(KeyCode.E))
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
                }
            }
            catch
            {
                if (hiding)
                {
                    PlaceItem(currentPatrolPoint.position);
                    hiding = false;
                }
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

            /*if (!hiding)
            {
                curIndex++;
                if (curIndex >= currentPoints.Count)
                {
                    curIndex = 0;
                }
                SwitchToPoint(curIndex);
            }*/
        }

        RaycastHit hit;
        if(Physics.Raycast(transform.position, playerObject.transform.position - transform.position, out hit, sightRange))
        {
            if(hit.collider.gameObject == playerObject)
            {
                if (throwables.Count > 0)
                {
                    curTime -= Time.deltaTime;
                    if (curTime <= 0)
                    {
                        curTime = timeToThrow;
                        GameObject toThrow = throwables[0];
                        toThrow.transform.LookAt(playerObject.transform.position);
                        toThrow.GetComponent<Rigidbody>().AddForce(toThrow.transform.forward * throwForce, ForceMode.Impulse);
                    }
                }
            }
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
        /*patrolPoints.Remove(patrolPoints[index]);
        if(curIndex == index)
        {
            if(curIndex >= patrolPoints.Count)
            {
                curIndex = 0;
            }
            SwitchToPoint(curIndex);
        }*/

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
        /*
        patrolPoints.Clear();
        foreach (Transform item in endGamePoints)
        {
            patrolPoints.Add(item);
        }
        agent.SetDestination(patrolPoints[0].position);
        currentPatrolPoint = patrolPoints[0];
        */
        currentTasks.Clear();
        currentPoints.Clear();
        /*foreach (TaskManager.Task item in endGameTasks)
        {
            currentTasks.Add(item);
        }
        foreach (List<Transform> item in endPointsPerTask)
        {
            currentPoints.Add(new List<Transform>());
            foreach (Transform trans in item)
            {
                currentPoints[currentPoints.Count - 1].Add(trans);
            }
        }*/

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
        hiding = true;
        int rand = Random.Range(0, hidingPlaces.Length);
        agent.SetDestination(hidingPlaces[rand].position);
        currentPatrolPoint = hidingPlaces[rand];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Light") && lightSourcesEffecting.Contains(other.gameObject) == false)
        {
            lightSourcesEffecting.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Light"))
        {
            lightSourcesEffecting.Remove(other.gameObject);
        }
    }

    private bool IsInLight()
    {
        foreach (GameObject light in lightSourcesEffecting)
        {
            RaycastHit hit;
            if(Physics.Raycast(light.transform.position, transform.position - light.transform.position, out hit, LayerMask.NameToLayer("Ignore Raycast")))
            {
                //Debug.Log(hit.collider.gameObject);
                if (hit.collider.gameObject == gameObject) return true;
            }
        }
        return false;
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
