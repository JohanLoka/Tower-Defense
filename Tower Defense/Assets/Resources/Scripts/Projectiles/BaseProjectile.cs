using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    //  BaseProjectile is just an container, projData holds all combat values.
    public Projectile_Data projData;

    //  Default damage, might be overriden
    public int Damage = 1;

    //  Data
    private Tower_Data shooterTower;

    public void Load(Tower_Data data)
    {
        this.shooterTower = data;
        this.projData = data.projectileData;
        this.Damage = (int)data.Damage;

        StartCoroutine(_timeDestroy());
    }

    protected virtual IEnumerator _timeDestroy()
    {
        yield return new WaitForSeconds(3f);
        Destroy(this.gameObject);
    }

    //  For safety and GC
    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    //  WIP
    public virtual void ApplyEffects()
    {
        if (projData.typeOfDebuff == DebuffType.Type.Splash)
        {
            GetEnemiesWithinAoE(3.2f).ForEach(e =>
            {
                e.OnHit(this);
            });
        }

        if (projData.typeOfDebuff == DebuffType.Type.Slow)
        {
            GetEnemiesWithinAoE().ForEach(e =>
            {
                e.moveHandler.OnSlow();
            });
        }
    }

    protected List<BaseEntity> GetEnemiesWithinAoE(float radius = 2.5f)
    {
        List<BaseEntity> enemies = new List<BaseEntity>();
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, radius, GameManager.Instance.EnemyLayer);

        int i = 0;
        while (i < hitColliders.Length)
        {
            enemies.Add(hitColliders[i].GetComponent<BaseEntity>());
            i++;
        }

        return enemies;
    }

    protected bool ShouldDamageOnSingleHit()
    {
        //  Add types here that should not damage on impacted enemy directly
        List<DebuffType.Type> excludes = new List<DebuffType.Type>{
            DebuffType.Type.Splash,
            DebuffType.Type.Amplify
        };
        return !excludes.Contains(projData.typeOfDebuff);
    }

    public virtual void OnEntityHit(BaseEntity target)
    {
        ApplyEffects();

        if (ShouldDamageOnSingleHit())
            target.OnHit(this);

        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Enemy" || shooterTower.GroundOnly) return;

        BaseEntity enemy = other.gameObject.GetComponent<BaseEntity>();

        if (!enemy) return;

        OnEntityHit(enemy);
    }

}
