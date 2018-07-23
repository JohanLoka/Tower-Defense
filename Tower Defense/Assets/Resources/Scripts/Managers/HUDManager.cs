using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class HUDManager : MonoBehaviour
{

    public static HUDManager Instance;
    private StatsManager statsManager;

    public GameObject HealthBarCanvas;

    [Header("World Text HUD")]
    public Text LifeText;
    public TMP_Text WaveText;

    [Header("Text UI")]
    public Text GoldText;
    public Text EnergyText;

    private Transform TextContainer;

    [Header("Resource Collecting")]
    public Transform resourceCanvas;
    public RectTransform goldIcon;

    [Header("Prefabs")]
    public GameObject animatedGold;

    [Header("Collecting Animation")]
    public Ease animationEase = Ease.Linear;
    private float collectAnimationDuration = .75f;

    void Awake()
    {
        Instance = this;

        //  Hookup Events
        EventsSubscribe();
    }
    void Start()
    {
        statsManager = StatsManager.Instance;

#if UNITY_ANDROID && !UNITY_EDITOR
        HealthBarCanvas.SetActive(false);
#endif
    }

    private void EventsSubscribe()
    {
        WaveManager.onEnemyReachedEnd += OnEnemyReachedEnd;
        WaveManager.onWaveIncreased += OnWaveIncreased;
        WaveManager.onEnemyKilled += OnEnemyKilled;

        StatsManager.onGoldChanged += OnGoldChanged;

        MapManager.onMapLoaded += OnMapLoaded;
    }

    private void InitTextComponents()
    {
        TextContainer = GameObject.Find("__Map").transform.Find(MapManager.Instance.SelectedMap + "(Clone)").transform.Find("TextPoints").transform;

        //Transform LifeTextTransform = TextContainer.Find("LifePoint");
        Transform WaveTextTransform = TextContainer.Find("WavePoint");

        // LifeText.transform.position = LifeTextTransform.position;
        WaveText.transform.position = WaveTextTransform.position;

        // LifeText.transform.SetParent(LifeTextTransform);
        WaveText.transform.SetParent(WaveTextTransform);
    }

    private void OnMapLoaded()
    {
        //  Hookup Components
        InitTextComponents();
    }

    #region UIEvents

    /*
        These methods respond to delegate events from other classes
     */

    private void OnEnemyKilled(BaseEntity entity)
    {
        CollectResource(StatsManager.ResourceType.Gold, 1, entity.gameObject);
    }

    private void OnEnemyReachedEnd(int life)
    {
        int val = life < 0 ? 0 : life;
        LifeText.text = string.Format("{0}", val);
    }

    private void OnWaveIncreased(int wave)
    {
        WaveText.text = string.Format("Wave \n{0}", wave);
    }

    private void OnGoldChanged(int count)
    {
        if (GoldText)
            GoldText.text = string.Format("x{0}", count);
    }

    private void OnEnergyChanged(int count)
    {
        EnergyText.text = string.Format("x{0}", count);
    }

    #endregion


    #region Collecting

    private void CollectResource(StatsManager.ResourceType type, int count, GameObject spawnedBy)
    {
        switch (type)
        {
            case StatsManager.ResourceType.Gold: Execute(type, count, animatedGold, spawnedBy, goldIcon); break;
            case StatsManager.ResourceType.Energy: Execute(type, count, animatedGold, spawnedBy, goldIcon); break;
        }
    }

    private void Execute(StatsManager.ResourceType type, int count, GameObject pref, GameObject spawnedBy, RectTransform target)
    {
        //  Create clone
        RectTransform clone = Instantiate(pref, resourceCanvas, false).GetComponent<RectTransform>();

        //  Anchor UI to world position
        clone.anchorMin = Camera.main.WorldToViewportPoint(spawnedBy.transform.position);
        clone.anchorMax = clone.anchorMin;

        //  Handle offsets of different sizes of phones
        clone.anchoredPosition = clone.localPosition;
        clone.anchorMin = new Vector2(0.5f, 0.5f);
        clone.anchorMax = clone.anchorMin;

        //  Set target as parent, lerp to zero
        clone.SetParent(target);

        //  Start animation
        clone
        .DOAnchorPos(Vector3.zero, collectAnimationDuration)
        .SetEase(animationEase)
        .OnComplete(() =>
        {
            //  On item reached end, do punch animation and update resources
            target
            .DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.4f)
            .OnComplete(() => { target.localScale = Vector2.one; })
            .Play();

            //  Notify we have collected resource
            UpdateResourceValueBy(type, count);

            //  Destroy item
            Destroy(clone.gameObject);
        })
        .Play();
    }


    private void UpdateResourceValueBy(StatsManager.ResourceType type, int value)
    {
        switch (type)
        {
            case StatsManager.ResourceType.Gold: statsManager.currentGold += value; break;
            case StatsManager.ResourceType.Energy: statsManager.currentEnergy += value; break;
        }
    }

    #endregion
}
