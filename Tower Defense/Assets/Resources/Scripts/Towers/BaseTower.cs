using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTower : MonoBehaviour
{
    public Tower_Data towerData;
    protected SpawnManager spawnManager;
    protected TowerManager towerManager;

    [Header("Tracking")]
    public BaseEntity currentTarget;

    //  States
    public bool isActive;

    //  Components
    protected Transform TopPart, BottomPart, Nozzle;

    //  Static Data
    private readonly float targetScanInterval = .5f;

    #region Props

    protected bool HasTarget
    {
        get
        {
            return currentTarget ? currentTarget.Health > 0 : false;
        }
    }

    protected float DistanceToTarget
    {
        get
        {
            return Vector3.Distance(this.transform.position, currentTarget.transform.position);
        }
    }

    //When Tower has no target, look "ahead"
    private Vector3 GetDeadZone
    {
        get
        {
            return this.transform.position - new Vector3(0, 0, 2);
        }
    }

    protected virtual bool WithingRange
    {
        get
        {
            return HasTarget ? DistanceToTarget <= towerData.Range : false;
        }
    }

    #endregion

    #region Init
    void Awake()
    {
        //  Main
        TopPart = this.transform.Find("Tower_Top");
        BottomPart = this.transform.Find("Tower_Base");

        //  Components
        Nozzle = TopPart.Find("Nozzle");
    }

    void Start()
    {
        spawnManager = SpawnManager.Instance;
        towerManager = TowerManager.Instance;

        isActive = true;
        towerManager.TowerAdded(this);

        //Debugging only
        StartCoroutine(firepProcess());

        // Start scanning for Enemys
        StartCoroutine(TargetScanner());
    }

    #endregion

    #region Tracking
    protected virtual void RotateTowards(Vector3 targetPos)
    {
        //  Ignore y
        Vector3 trackPos = targetPos;
        trackPos.y = TopPart.transform.position.y;

        //  Add Lerping

        this.TopPart.LookAt(trackPos);
    }

    private IEnumerator TargetScanner()
    {
        while (isActive)
        {
            currentTarget = getClosestEnemy();
            yield return new WaitForSeconds(targetScanInterval);
        }
    }

    protected virtual BaseEntity getClosestEnemy()
    {
        //maybe return the 3 closest and chose the one with the lowest hp?
        return towerManager.GetClosestEnemy(this);
    }
    #endregion

    #region Combat
    
    protected virtual void Fire()
    {
        GameObject proj = spawnManager.Spawn(towerData.projectileData.Projectile, Nozzle.transform.position, Nozzle.transform.rotation);
        proj.GetComponent<BaseProjectile>().Load(towerData);

        SetAim();

        //  Apply Speed
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        rb.velocity = Nozzle.forward * towerData.Power;
    }

    private void SetAim()
    {
        //Calculate aim based on speed and rotation of enemies.
        float predictAmount = 25;
        float predict = currentTarget.moveHandler.MovementSpeed / (predictAmount / DistanceToTarget);
        Vector3 trackPos = currentTarget.transform.position + (currentTarget.transform.forward * predict);

        trackPos.y += DistanceToTarget * .13f;

        Nozzle.LookAt(trackPos);
    }

    #endregion

    void Update()
    {
        if (!HasTarget) return;

        if (WithingRange && isActive)
        {
            //Track Target
            RotateTowards(currentTarget.transform.position);

            //Calc Firing
        }
        else RotateTowards(GetDeadZone);
    }

    //  Debug and testing function only!
    private IEnumerator firepProcess()
    {
        if (WithingRange && HasTarget)
            Fire();

        yield return new WaitForSeconds(towerData.RoF);

        if (isActive)
            StartCoroutine(firepProcess());
    }
}
