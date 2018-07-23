using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawnArea : MonoBehaviour
{
    //  Load Components
    void Start()
    {
        SpawnManager.Instance.SpawnAreas.Add(this.transform);
    }
}
