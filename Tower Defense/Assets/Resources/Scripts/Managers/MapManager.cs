using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; set; }

    //  Database accessors
    private readonly string SELECTED_MAP = "SELECTED_MAP";

    //  Fallback
    private readonly string FALLBACK_MAP = "Template Map";

    //  Play Scene
    private readonly string PLAY_SCENE = "SampleScene";

    //  Map prop	
    public string SelectedMap
    {
        get
        {
            return PlayerPrefs.HasKey(SELECTED_MAP) ? PlayerPrefs.GetString(SELECTED_MAP) : FALLBACK_MAP;
        }
        private set
        {
            PlayerPrefs.SetString(SELECTED_MAP, value);
        }
    }

    //  Populated in Editor with all maps gameobjects
    public List<GameObject> Maps = new List<GameObject>();

    #region Delegates

    public delegate void OnMapLoaded();
    public static event OnMapLoaded onMapLoaded;

    #endregion

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "StartMenu")
            LoadMap();
    }

    //  Here we list all maps that are ready
    private List<string> AvailableMaps = new List<string>{
        "Template Map",
        "Test_Map_1",
        "Test_Map_2"
    };


    #region Saving

    public void SetMap(string MapName)
    {
        SelectedMap = MapName;
    }

    #endregion

    #region Loading

    private void LoadMap()
    {
        Transform container = GameObject.Find("__Map").transform;
        GameObject selectedMap = Maps.Where(item => item.name == SelectedMap).First();
        GameObject map = Instantiate(selectedMap, container.transform.position, Quaternion.identity) as GameObject;

        map.transform.SetParent(container);

        if (onMapLoaded != null) onMapLoaded();
    }

    #endregion

    public bool IsMapCorrect(string mapName)
    {
        return AvailableMaps.Contains(mapName);
    }
}
