using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance { get; set; }
    public WaveManager waveManager { get; set; }

    public List<BaseTower> activeTowers = new List<BaseTower>();

    #region Delegates

    public delegate void OnTowerAdded(BaseTower tower);
    public static event OnTowerAdded onTowerAdded;

    #endregion

    void Awake()
    {
        Instance = this;

        // Hookup Events
        EventsSubscribe();
    }

    void Start()
    {
        waveManager = WaveManager.Instance;
    }

    private void EventsSubscribe()
    {
        WaveManager.onGameLost += OnGameLost;
    }

    #region Triggers

    public void TowerAdded(BaseTower tower)
    {
        activeTowers.Add(tower);

        //  Delegates
        if (onTowerAdded != null) onTowerAdded(tower);
    }

    private void OnGameLost()
    {
        activeTowers.ForEach(t => t.isActive = false);

        //  Maybe add some ui particles over each tower indicating inactive?
    }

    #endregion

    public BaseEntity GetClosestEnemy(BaseTower tower)
    {
        float closest = 999f;
        BaseEntity target = null;

        WaveManager.Instance.Entitys.ForEach(e =>
        {
            float d = Vector3.Distance(tower.transform.position, e.transform.position);
            if (d < closest)
            {
                closest = d;
                target = e;
            }
        });

        return target;
    }

}
