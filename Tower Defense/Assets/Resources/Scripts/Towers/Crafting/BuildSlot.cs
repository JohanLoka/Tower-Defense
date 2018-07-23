using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildSlot : MonoBehaviour
{
    private BuildManager buildManager;

    public enum State
    {
        Empty,
        Built
    }
    public State state = State.Empty;

    //  If slot has a tower 
    public BaseTower baseTower;

    //  Component where tower should actually spawn
    public Transform spawnPoint { get; set; }

    void Start()
    {
        buildManager = BuildManager.Instance;

        //  Child transform
        spawnPoint = transform.Find("spawnPoint");
    }

    public void OnClick()
    {
        switch (state)
        {
            case State.Empty:
                buildManager.OnClickBuildSlot(this);
                break;
            case State.Built:
                buildManager.OnClickTower(baseTower);
                break;
        }

    }

    //   Works on Touch also
    private void OnMouseDown()
    {
        OnClick();
    }

}
