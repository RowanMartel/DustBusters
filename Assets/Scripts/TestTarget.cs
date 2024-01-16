using UnityEngine;
using UnityEngine.AI;

public class TestTarget : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform[] patrolPoints;
    public Transform currentPatrolPoint;
    public int curIndex;

    public float distToSwitch;

    // Start is called before the first frame update
    void Start()
    {
        SwitchToPoint(0);
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(currentPatrolPoint.position);
        if (distToSwitch > Vector3.Distance(transform.position, currentPatrolPoint.position))
        {
            curIndex++;
            if (curIndex >= patrolPoints.Length)
            {
                curIndex = 0;
            }
            SwitchToPoint(curIndex);
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
