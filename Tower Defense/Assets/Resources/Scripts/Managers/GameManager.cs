using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; set; }

    public LayerMask EnemyLayer;
    public Transform EnemyTargetPoint;
    
    public Canvas canvas;

    #region Delegates

    public delegate void OnGameStart();
    public static event OnGameStart onGameStart;

    #endregion

    void Awake()
    {
        Instance = this;

        //  Hookup Events
        EventsSubscriber();
    }

    private void EventsSubscriber()
    {
        WaveManager.onGameLost += OnGameOver;
    }


    public void OnGameOver()
    {
        //  Show UI for Gameover
    }

    public void StartGame()
    {
        //  Delegate
        if (onGameStart != null) onGameStart();
    }

}
