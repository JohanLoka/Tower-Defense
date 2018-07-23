using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TowerCraftItem : MonoBehaviour
{
    //  Injected Tower to craft upon click
    public Tower_Data towerData;

    public BuildManager buildManager { get; set; }

    public TextMeshProUGUI CostText { get; set; }
    public TextMeshProUGUI TitleText { get; set; }
    
    void Awake()
    {
        CostText = transform.Find("Cost").gameObject.GetComponent<TextMeshProUGUI>();
        TitleText = transform.Find("Title").gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        buildManager = BuildManager.Instance;

        LoadData();
    }

    private void LoadData()
    {
        if (!towerData) return;

        CostText.text = string.Format("Cost: {0}", towerData.BuildCost);
        TitleText.text = towerData.TowerName;
    }

    private bool CanCraft()
    {
        //  Apply any upgrades here for discounts
        return StatsManager.Instance.currentGold >= towerData.BuildCost;
    }

    public void OnClick()
    {
        if (!CanCraft()) return;

        buildManager.CraftTower(this);
    }
}
