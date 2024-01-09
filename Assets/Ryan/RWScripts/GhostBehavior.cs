using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostBehavior : MonoBehaviour
{
    [Header("Navigation")]
    public NavMeshAgent agent;

    public List<Transform> patrolPoints;
    public Transform currentPatrolPoint;
    public int curIndex;

    public float distToSwitch;

    [Header("Attack")]
    public GameObject victim;
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
    public GameObject key;
    public Transform[] hidingPlaces;

    // Start is called before the first frame update
    void Start()
    {
        SwitchToPoint(0);
        curTime = timeToThrow;
        hiding = false;
    }

    // Update is called once per frame
    void Update()
    {

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
            if (hiding)
            {
                PlaceItem(currentPatrolPoint.position);
                hiding = false;
            }
            if(currentPatrolPoint == key.transform)
            {
                PickUpItem(key);
                ChooseHidingPlace();
            }
            if (!hiding)
            {
                curIndex++;
                if (curIndex >= patrolPoints.Count)
                {
                    curIndex = 0;
                }
                SwitchToPoint(curIndex);
            }
        }

        RaycastHit hit;
        if(Physics.Raycast(transform.position, victim.transform.position - transform.position, out hit, sightRange))
        {
            if(hit.collider.gameObject == victim)
            {
                if (throwables.Count > 0)
                {
                    curTime -= Time.deltaTime;
                    if (curTime <= 0)
                    {
                        curTime = timeToThrow;
                        GameObject toThrow = throwables[0];
                        toThrow.transform.LookAt(victim.transform.position);
                        toThrow.GetComponent<Rigidbody>().AddForce(toThrow.transform.forward * throwForce, ForceMode.Impulse);
                    }
                }
            }
        }
        
    }

    void SwitchToPoint(int index)
    {
        if (index < patrolPoints.Count)
        {
            currentPatrolPoint = patrolPoints[index];
            agent.SetDestination(currentPatrolPoint.position);
            curIndex = index;
        }
    }

    public void RemovePatrolPoint(int index)
    {
        patrolPoints.Remove(patrolPoints[index]);
        if(curIndex == index)
        {
            if(curIndex >= patrolPoints.Count)
            {
                curIndex = 0;
            }
            SwitchToPoint(curIndex);
        }
    }

    public void AddPatrolPoint(Transform point)
    {
        patrolPoints.Add(point);
    }
    
    public void EnterEndGame()
    {
        patrolPoints.Clear();
        AddPatrolPoint(key.transform);
        AddPatrolPoint(victim.transform);
        agent.SetDestination(patrolPoints[0].position);
        currentPatrolPoint = patrolPoints[0];
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
        curHeldItem = null;
    }

    void PlaceItem(Vector3 pos)
    {
        GameObject item = curHeldItem;
        DropItem();
        item.transform.position = pos;
    }

    public void ChooseHidingPlace()
    {
        hiding = true;
        int rand = Random.Range(0, hidingPlaces.Length);
        agent.SetDestination(hidingPlaces[rand].position);
        currentPatrolPoint = hidingPlaces[rand];
    }

}
