using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    [SerializeField] private int _golds = 0;
    [SerializeField] private string _lastSaveDate;
    [SerializeField] private string _saveVersion = "1.0";

    private float _masterVolume = 1f;
    private float _musicVolume = 0.5f;
    private float _sfxVolume = 0.5f;

    public void SavePlayerPrefs()
    {
        PlayerPrefs.SetFloat("MasterVolume", _masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", _sfxVolume);
        PlayerPrefs.Save();
    }

    public void LoadPlayerPrefs()
    {
        _masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        _musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        _sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
    }

    public int Golds => _golds;
    public string LastSaveDate => _lastSaveDate;

    public void AddGolds(int amount)
    {
        _golds += amount;
    }
    public void RemoveGolds(int amount)
    {
        _golds -= amount;
    }

    public string SetLastSaveDate()
    {
        _lastSaveDate = DateTime.Now.ToString();
        return _lastSaveDate;
    }
}
