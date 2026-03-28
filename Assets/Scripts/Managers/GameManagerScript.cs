using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GameManager", menuName = "ScriptableObjects/Managers/Game", order = 1)]
public class GameManager : ScriptableObject
{
    [Header("Managers")]
    [SerializeField] private PlayerDataManager _playerDataManager;

    [Header("Settings")]
    [SerializeField] private int _secondsToPlay = 60;
    private int _currentSeconds = 0;
    private int _killedEnemies = 0;
    private GlobalLightScript _globalLight;
    private bool _isGamePaused = false;
    private int _golds = 0;

    private List<Upgrade> _sessionUpgrades = new();
    private Dictionary<string, Upgrade>_definitivesUpgrades = new();

    public void LoadGame()
    {
        ResetSessionUgrades();
        _definitivesUpgrades.Clear();
        _playerDataManager.LoadPlayerData(_definitivesUpgrades);
        _golds = 0;
    }

    public void SaveGame()
    {

    }

    public void ResetSessionUgrades()
    {
        _sessionUpgrades.Clear();
    }

    public int GetSecondsLeft()
    {
        return _secondsToPlay - _currentSeconds;
    }

    public void AddSeconds(int seconds)
    {
        _currentSeconds += seconds;
        if (_currentSeconds > _secondsToPlay) _currentSeconds = _secondsToPlay;
        else if (_currentSeconds < 0) _currentSeconds = 0;
    }

    public void ResetSeconds()
    {
        _currentSeconds = 0;
    }

    public void AddKilledEnemy() => _killedEnemies++;
    
    public int GetKilledEnemies() => _killedEnemies;

    public void ResetKilledEnemies() => _killedEnemies = 0;

    public int GetTimePlayed() => _currentSeconds;

    public int GetSessionDuration() => _secondsToPlay;

    public int GetGolds() => _golds;

    public void AddGolds(int golds) { _golds += golds; }

    public void StartNewSession()
    {
        ResetSessionUgrades();
        ResetSeconds();
        ResetKilledEnemies();
        _playerDataManager.LoadPlayerData(_definitivesUpgrades);
    }

    public void SetGlobalLightScript(GlobalLightScript globalLight)
    {
        _globalLight = globalLight;
    }

    public void SetGlobalLight(float intensity)
    {
        if (_globalLight != null) _globalLight.SetGlobalLightIntensity(intensity);
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
        _golds /= 2;
    }

    public void OnBuyDefinitiveUpgrade(Upgrade upgrade)
    {
        if (_golds > upgrade.GetUpgradeData().DefinitiveCost)
        {
            _golds -= upgrade.GetUpgradeData().DefinitiveCost;
            if (_definitivesUpgrades.ContainsKey(upgrade.GetId())) _definitivesUpgrades[upgrade.GetId()].GetUpgradeData().CombineData(upgrade);
            else _definitivesUpgrades.Add(upgrade.GetId(), upgrade);
            ActionsManager.OnSelectDefinitiveUpgrade?.Invoke(upgrade);
        }
    }
}
