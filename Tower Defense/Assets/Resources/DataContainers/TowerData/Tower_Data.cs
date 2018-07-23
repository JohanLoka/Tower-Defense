using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tower", menuName = "Tower")]
public class Tower_Data : ScriptableObject
{
    public string TowerName;
    public string TowerNameDatabase;
    
    [Header("Cost")]
    public int BuildCost;
    public int[] UpgradeCosts;

    [Header("Combat")]
    public float Damage;
    public float Power;

    [Range(.1f, 10)]
    public float RoF;

    [Range(3, 30)]
    public float Range;

    public DamageType typeOfDamage;

    [Header("Tracking")]

    public bool GroundOnly;
    [Range(1, 10)]
    public float TrackingSpeed;

    [Header("Tower")]
    public GameObject TowerPrefab;

    [Header("Projectile")]
    public Projectile_Data projectileData;
}
