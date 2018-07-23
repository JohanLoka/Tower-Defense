using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestAi : MonoBehaviour
{

    public GameManager gameManager { get; set; }
    public Transform TargetPos;
    private Vector3 StartPos;
    private bool Forward = true;
    public Animator anim;

    public NavMeshAgent navAgent { get; set; }
    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        StartPos = this.transform.position;
    }

    void Start()
    {
        StartCoroutine(checkProgress());
    }


    public void SetDestination(Vector3 destination)
    {
        if (navAgent.isOnNavMesh)
        {
            navAgent.SetDestination(destination);
            navAgent.isStopped = false;
        }

        if (anim)
        {
            anim.SetBool("Moving", true);
            anim.SetBool("Running", true);
        }
    }

    private IEnumerator checkProgress()
    {
        yield return new WaitForSeconds(1f);

        if (!TargetPos)
            TargetPos = GameManager.Instance.EnemyTargetPoint;

        SetDestination(TargetPos.position);
    }
}
