using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private int _pixel_per_unit = 16;
    [SerializeField] private GameData _gameData;

    private int _countSeconds = 0;
    private int _countKills = 0;
    private bool _isGamePaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {

    }

    public void ResetSessionUgrades()
    {

    }

    public int PIXELS_PER_UNIT => _pixel_per_unit;

    public void AddKilledEnemy() => _countKills++;
    
    public int GetKilledEnemies() => _countKills;

    public void ResetKilledEnemies() => _countKills = 0;

    public int GetTimePlayed() => _countSeconds;

    public int GetSessionDuration() => _gameData.SecondsToPlay;

    public void StartNewSession()
    {
        ResetSessionUgrades();
        ResetKilledEnemies();
    }

    public void SetGlobalLightScript(GlobalLightScript globalLight)
    {
        //_globalLight = globalLight;
    }

    public void SetGlobalLight(float intensity)
    {
        //if (_globalLight != null) _globalLight.SetGlobalLightIntensity(intensity);
    }

    public void TogglePause(bool pause)
    {
        _isGamePaused = pause;
        if (pause) Time.timeScale = 0f;
        else Time.timeScale = 1f;
    }

    public bool IsGamePaused()
    {
        return _isGamePaused;
    }

    public void OnPlayerKilled()
    {

    }

    public void OnBuyDefinitiveUpgrade(Stats upgrade)
    {
        /*if (_golds > upgrade.GetUpgradeData().DefinitiveCost)
        {
            _golds -= upgrade.GetUpgradeData().DefinitiveCost;
            if (_definitivesUpgrades.ContainsKey(upgrade.GetId())) _definitivesUpgrades[upgrade.GetId()].GetUpgradeData().CombineData(upgrade);
            else _definitivesUpgrades.Add(upgrade.GetId(), upgrade);
            ActionsManager.OnSelectDefinitiveUpgrade?.Invoke(upgrade);
        }*/
    }

    public int GetProcs(int procChance)
    {
        int procs = 0;
        while (UnityEngine.Random.Range(0, 100) < procChance)
        {
            procs++;
            procChance -= 100;
        }
        return procs;
    }
}
