using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostBehavior : MonoBehaviour
{
    [Header("Navigation")]
    public NavMeshAgent agent;

    public Transform[] patrolPoints;
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

    // Start is called before the first frame update
    void Start()
    {
        SwitchToPoint(0);
        curTime = timeToThrow;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(currentPatrolPoint.position);
        if(distToSwitch > Vector3.Distance(transform.position, currentPatrolPoint.position))
        {
            curIndex++;
            if(curIndex >= patrolPoints.Length) {
                curIndex = 0;
            }
            SwitchToPoint(curIndex);
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
        if (index < patrolPoints.Length)
        {
            currentPatrolPoint = patrolPoints[index];
            agent.SetDestination(currentPatrolPoint.position);
            curIndex = index;
        }
    }

}
