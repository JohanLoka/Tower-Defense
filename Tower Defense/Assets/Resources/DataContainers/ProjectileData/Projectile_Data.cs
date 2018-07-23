using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile", menuName = "Projectile")]
public class Projectile_Data : ScriptableObject
{
public string Name;
public string NameDatabase;

[Header("Data")]
public float Duration;
public DebuffType.Type typeOfDebuff;

[Header("Visual")]
public GameObject Projectile;
}
