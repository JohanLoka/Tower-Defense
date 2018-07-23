using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    public static SpawnManager Instance { get; set; }

    //  Used to place objects for runtime-project organization
    public Transform ProjectileContainer;
    public Transform EnemyContainer;
    public Transform FXContainer;
    public Transform GenericContainer;

    [Header("Enemy Spawn Areas")]
    public List<Transform> SpawnAreas = new List<Transform>();

    void Awake()
    {
        Instance = this;
    }

    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rotation)
    {
        GameObject item = Instantiate(prefab, pos, rotation) as GameObject;
        item.transform.SetParent(getCorrectParent(item.tag));
        return item;
    }

    private Transform getCorrectParent(string tag)
    {
        switch (tag)
        {
            case "Projectile": return ProjectileContainer;
            case "Enemy": return EnemyContainer;
            case "FX": return FXContainer;
            default: return GenericContainer;
        }
    }

    public Vector3 GetRandomSpawn(Transform area)
    {
        Vector3 spawnPos = area.position;
        spawnPos.x += Random.Range(-3, 3);
        spawnPos.z += Random.Range(-3, 3);

        return spawnPos;
    }
}
