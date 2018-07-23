using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntity : MonoBehaviour
{
    public EntityMovementHandler moveHandler { get; set; }
    public EntityAnimHandler animHandler { get; set; }

    protected WaveManager waveManager { get; set; }

    [Header("Stats")]
    public int Health;
    public int MaxHealth;

    private bool ReachedGoal;

    //  Data
    private float corpseDuration = 4.5f;

    void Awake()
    {
        moveHandler = GetComponent<EntityMovementHandler>();
        animHandler = GetComponent<EntityAnimHandler>();

        //  If we dont have any animation, remove corpse faster.
        if (!animHandler.anim)
            corpseDuration = .25f;
    }

    void Start()
    {
        waveManager = WaveManager.Instance;
        OnSpawn();
    }
    #region Events

    public virtual void OnDeath()
    {
        GetComponent<Collider>().enabled = false;

        //  Death-animation
        animHandler.OnDeath();

        //  Stop Moving
        moveHandler.OnDeath();

        //  Notify Manager
        waveManager.EnemyKilled(this);

        //  Remove corpse after delay
        StartCoroutine(despawnCorpse());

        if (ReachedGoal) return;

        //  Reward Gold

        //  Count Towards Wave Count

        //  Play Sound
    }

    private IEnumerator despawnCorpse()
    {
        yield return new WaitForSeconds(corpseDuration);
        Destroy(gameObject);
    }


    public virtual void OnSpawn()
    {
        //  Test Only - Mutations
        if (waveManager)
        {
            if (waveManager.Wave >= 3)
            {
                if (Random.Range(0, 100) >= 90)
                {
                    //  Medium dawg
                    this.transform.localScale *= 1.6f;
                    this.Health *= 4;
                }
            }
            if (waveManager.Wave >= 5)
            {
                if (Random.Range(0, 100) >= 96)
                {
                    //  Big dawg
                    this.transform.localScale *= 2.2f;
                    this.Health *= 8;
                }
            }
        }


        waveManager.Entitys.Add(this);
        MaxHealth = Health;
    }

    public virtual void OnGoalReached()
    {
        waveManager.EnemyReachedGoal(this);
        ReachedGoal = true;
    }

    public virtual void OnHit(BaseProjectile projectile = null, int customDamage = 1)
    {
        int damage = customDamage;
        if (projectile)
        {
            damage = projectile.Damage;
            HandleDebuff(projectile.projData.typeOfDebuff);
        }

        Health -= damage;
    }
    #endregion

    #region States

    protected virtual void HandleDebuff(DebuffType.Type debuff)
    {
        switch (debuff)
        {
            case DebuffType.Type.Slow:
                moveHandler.OnSlow();
                break;
            case DebuffType.Type.DoT:
                //  Apply DoT
                break;
            case DebuffType.Type.Stun:
                //  Apply Stun
                break;
            case DebuffType.Type.Amplify:
                //  Apply Armor Reducer
                break;
        }
    }

    #endregion
}
