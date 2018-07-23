using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EntityMovementHandler : MonoBehaviour
{

    protected EntityAnimHandler animHandler;
    protected NavMeshAgent navAgent;
    protected Rigidbody rb;
    
    protected float OriginalSpeed;
    public float Speed;

    #region Props

    public Quaternion Rotation { get { return transform.rotation; } }
    public float MovementSpeed { get { return navAgent.speed; } }

    #endregion

    void Awake()
    {
        animHandler = GetComponent<EntityAnimHandler>();
        navAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        OriginalSpeed = Speed;
    }

    void Start()
    {
        animHandler.ChangeSpeed(Speed);
    }

    #region States

    public virtual void OnStartMove()
    {

    }

    public virtual void OnStopMove()
    {

    }

    public virtual void OnDeath()
    {
        navAgent.isStopped = true;
        navAgent.enabled = false;

        rb.useGravity = false;
        rb.velocity = Vector3.zero;

        StopAllCoroutines();
    }

    #endregion

    #region Events

    public virtual void ChangeSpeed(float newSpeed)
    {
        animHandler.ChangeSpeed(newSpeed);
        navAgent.speed = (newSpeed / Speed);
        Speed = newSpeed;
    }

    public virtual void OnSlow()
    {
        StopCoroutine(_setSlow());
        StartCoroutine(_setSlow());
    }

    protected virtual IEnumerator _setSlow()
    {
        ChangeSpeed(OriginalSpeed / 2);
        yield return new WaitForSeconds(4f);
        ChangeSpeed(OriginalSpeed);
    }

    #endregion
}
