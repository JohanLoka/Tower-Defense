using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{

    public static StatsManager Instance { get; set; }

    //  All types of resources that should be shown on screen
    public enum ResourceType { Gold, Energy, Life };

    //  Access refs to value stored in localstorage
    private readonly string GEM_ACCESSOR = "CURRENT_PLAYER_GEMS";
    private readonly string IRON_ACCESSOR = "CURRENT_PLAYER_IRON";

    //  Non currency refs
    private readonly string STATS_INITIALIZED = "HAS_INIT_STATS";

    [Header("Settings")]
    public int startingGold;
    public int startingEnergy;

    #region Props

    private int _currentGold;
    public int currentGold
    {
        get
        {
            return _currentGold;
        }
        set
        {
            _currentGold = value;

            //  Notify subscribers
            if (onGoldChanged != null) onGoldChanged(value);
        }
    }

    private int _currentEnergy;
    public int currentEnergy
    {
        get
        {
            return _currentEnergy;
        }
        set
        {
            _currentEnergy = value;

            //  Notify subscribers
            if (onEnergyChanged != null) onEnergyChanged(value);
        }
    }

    #endregion

    #region Delegates

    public delegate void OnGoldChanged(int count);
    public static event OnGoldChanged onGoldChanged;

    public delegate void OnEnergyChanged(int count);
    public static event OnEnergyChanged onEnergyChanged;

    #endregion

    void Awake()
    {
        Instance = this;

        // Only set this first time
        if (!PlayerPrefs.HasKey(STATS_INITIALIZED))
            InitialSetup();
    }

    void Start()
    {
        //  Set starting values
        currentEnergy = startingEnergy;
        currentGold = startingGold;
    }

    private void InitialSetup()
    {
        //  Only run this once
        PlayerPrefs.SetInt(STATS_INITIALIZED, 1);
    }
}
