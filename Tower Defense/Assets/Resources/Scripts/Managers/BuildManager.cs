using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoozyUI;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [Header("Slots")]
    public List<BuildSlot> slots = new List<BuildSlot>();


    [Header("Elements")]
    public UIElement towerBuilder;
    public UIElement towerUpgrader;

    [Header("World Text HUD")]
    public GameObject selectedSlotIndicator;

    //  Selected Objects
    public BaseTower selectedTower { get; private set; }
    public BuildSlot selectedSlot { get; private set; }

    //  States
    private bool builderWindowOpen;

    void Awake()
    {
        Instance = this;
    }

    #region Handlers

    private Vector3 getObjectUIPosition(Transform obj)
    {
        return obj.position + new Vector3(0, 1.5f, -.5f);
    }

    #endregion


    #region TowerBuilding

    public void CraftTower(TowerCraftItem itemCard)
    {
        if (!selectedSlot) return;

        GameObject tower = SpawnManager.Instance.Spawn(itemCard.towerData.TowerPrefab, selectedSlot.spawnPoint.position, Quaternion.identity);

        //  Populate correct data
        BaseTower baseTower = tower.GetComponent<BaseTower>();

        baseTower.towerData = itemCard.towerData;
        baseTower.transform.SetParent(selectedSlot.transform);

        //  Change state for future clicks
        selectedSlot.state = BuildSlot.State.Built;

        //  Assign current tower to slot
        selectedSlot.baseTower = baseTower;

        // Pay bills
        StatsManager.Instance.currentGold -= itemCard.towerData.BuildCost;

        CloseTowerBuilder();
    }

    #endregion

    #region ClickEvents

    public void OnClickBuildSlot(BuildSlot slot)
    {
        //  We cant accidently select a new tower if we are building another one
        if (builderWindowOpen) return;
        
        selectedSlot = slot;
        OpenTowerBuilder();
    }

    public void OnClickTower(BaseTower tower)
    {
        selectedTower = tower;
        OpenTowerUpgrader();
    }

    #endregion

    #region UIEvents

    public void CloseTowerUpgrader()
    {
        //  Close UI
        towerUpgrader.Hide(false);
    }

    public void CloseTowerBuilder()
    {
        builderWindowOpen = false;

        //  Close UI
        towerBuilder.Hide(false);

        //  Dont show selector
        selectedSlotIndicator.SetActive(builderWindowOpen);
    }

    public void OpenTowerBuilder()
    {
        if (builderWindowOpen) return;

        builderWindowOpen = true;

        //  Open UI
        towerBuilder.Show(false);

        //  Move indicator
        selectedSlotIndicator.SetActive(builderWindowOpen);
        selectedSlotIndicator.transform.position = selectedSlot.transform.position + new Vector3(0, 1, 0);

        //  Close other Huds
        CloseTowerUpgrader();
    }

    public void OpenTowerUpgrader()
    {
        if (!selectedTower || builderWindowOpen) return;

        //  Move HUD to correct place
        towerUpgrader.transform.parent.transform.position = getObjectUIPosition(selectedTower.transform);

        //  Open UI
        towerUpgrader.Show(false);
    }

    #endregion
}
