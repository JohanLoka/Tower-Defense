using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; set; }
    private SpawnManager spawnManager;
    private HUDManager hudManager;

    //  Active Enemys
    public List<BaseEntity> Entitys = new List<BaseEntity>();

    [Header("Enemys")]
    public GameObject BasicEnemy;

    #region Props
    protected int _life;
    public int Life
    {
        get { return _life; }
        private set { _life = value; }
    }

    protected int _wave;
    public int Wave
    {
        get { return _wave; }
        private set { _wave = value; }
    }

    #endregion

    #region Delegates

    public delegate void OnEnemyKill(BaseEntity enemy);
    public static event OnEnemyKill onEnemyKilled;

    public delegate void OnEnemyReachedEnd(int life);
    public static event OnEnemyReachedEnd onEnemyReachedEnd;

    public delegate void OnWaveIncreased(int wave);
    public static event OnWaveIncreased onWaveIncreased;

    public delegate void OnGameLost();
    public static event OnGameLost onGameLost;

    #endregion

    //  Counters
    private int KillsToAdvance;
    private int KillsThisRound;

    //  Static variables
    private readonly int spawnUnitsOverSeconds = 15;

    void Awake()
    {
        Instance = this;

        //  Hookup Events
        EventsSubscribe();
    }

    void Start()
    {
        hudManager = HUDManager.Instance;
        spawnManager = SpawnManager.Instance;
    }

    private void EventsSubscribe()
    {
        GameManager.onGameStart += OnGameStart;
    }

    #region Triggers

    private void OnGameStart()
    {
        //  Debug
        StartCoroutine(Debug_Test_New_Wave());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void EnemyReachedGoal(BaseEntity entity)
    {
        entity.Health = 0;
        Life--;

        //  Delegate
        if (onEnemyReachedEnd != null) onEnemyReachedEnd(Life);
        if (onGameLost != null && Life == 0) onGameLost();
    }

    public void EnemyKilled(BaseEntity enemy)
    {
        Entitys.Remove(enemy);

        //  Delegate 
        if (onEnemyKilled != null) onEnemyKilled(enemy);

        // Increment
        KillsThisRound++;
    }

    public void IncreaseWave()
    {
        Wave++;

        //  Delegate
        if (onWaveIncreased != null) onWaveIncreased(Wave);

        // Reset Data
        KillsThisRound = 0;
    }

    #endregion

    #region Debug

    //  Is called at GameStart and is running throughout the game until your life drops to 0
    private IEnumerator Debug_Test_New_Wave()
    {
        yield return new WaitForSeconds(1f);
        Life = 15;
        Wave = 0;

        while (Life > 0)
        {
            IncreaseWave();

            //  Spawn Enemys
            int count = Random.Range(Wave + 3, 8 + Wave);
            KillsToAdvance = count * spawnManager.SpawnAreas.Count;

            //  Spawn Enemies over X seconds
            float delay = (float)spawnUnitsOverSeconds / (float)count;
            for (int i = 0; i < count; i++)
            {
                spawnManager.SpawnAreas.ForEach(area =>
                {
                    spawnManager.Spawn(BasicEnemy, spawnManager.GetRandomSpawn(area), Quaternion.identity);
                });
                yield return new WaitForSeconds(delay);
            }

            //  Advance when we have sufficent kills.
            while (KillsThisRound < KillsToAdvance)
                yield return null;

            //  Small delay, maybe show some UI here?
            yield return new WaitForSeconds(1f);

            //  New wave!
        }
    }
    #endregion
}
