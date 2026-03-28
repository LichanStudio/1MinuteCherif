using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;


[CreateAssetMenu(fileName = "AudioManager", menuName = "ScriptableObjects/Managers/AudioManager", order = 1)]
public class AudioManager : ScriptableObject
{
    [Header("AudioSources")]
    public AudioSource MainMusicSource;
    public List<AudioSource> SoundsSources;

    [Header("AudioClips")]
    public List<AudioClip> SongsBattle;
    public List<AudioClip> SongsSaloon;
    public List<AudioClip> SoundsHit;
    public List<AudioClip> SoundsShoot;
    public List<AudioClip> SoundsBuy;

    private float _musicVolume = 1f;
    private float _soundsVolume = 1f;
    private int _lastSoundIndex = 0;

    public async void PlayMusicBattle()
    {
        if (MainMusicSource == null) return;
        if (SongsBattle == null || SongsBattle.Count == 0) return;
        await FadeOutAndPlay(SongsBattle[UnityEngine.Random.Range(0, SongsBattle.Count)], 0.5f);
    }

    public async void PlayMusicSaloon()
    {
        if (MainMusicSource == null) return;
        if (SongsSaloon == null || SongsSaloon.Count == 0) return;
        await FadeOutAndPlay(SongsSaloon[UnityEngine.Random.Range(0, SongsSaloon.Count)], 1f);
    }

    public void PlayHitSound()
    {
        if (SoundsHit == null || SoundsHit.Count == 0) return;
        if (SoundsSources == null || SoundsSources.Count == 0) return;
        _lastSoundIndex++;
        if (_lastSoundIndex >= SoundsSources.Count) _lastSoundIndex = 0;
        SoundsSources[_lastSoundIndex].PlayOneShot(SoundsHit[UnityEngine.Random.Range(0, SoundsHit.Count)], _soundsVolume * 0.4f);
    }

    public void PlayShootSound()
    {
        if (SoundsShoot == null || SoundsShoot.Count == 0) return;
        if (SoundsSources == null || SoundsSources.Count == 0) return;
        _lastSoundIndex++;
        if (_lastSoundIndex >= SoundsSources.Count) _lastSoundIndex = 0;
        SoundsSources[_lastSoundIndex].PlayOneShot(SoundsShoot[UnityEngine.Random.Range(0, SoundsShoot.Count)], _soundsVolume);
    }

    public void PlayBuySound()
    {
        if (SoundsBuy == null || SoundsBuy.Count == 0) return;
        if (SoundsSources == null || SoundsSources.Count == 0) return;
        _lastSoundIndex++;
        if (_lastSoundIndex >= SoundsSources.Count) _lastSoundIndex = 0;
        SoundsSources[_lastSoundIndex].PlayOneShot(SoundsBuy[UnityEngine.Random.Range(0, SoundsBuy.Count)], _soundsVolume);
    }

    public async Task FadeOutAndPlay(AudioClip nextClip, float duration)
    {
        // --- FADE OUT ---
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            MainMusicSource.volume = Mathf.Lerp(_musicVolume, 0, currentTime / duration);
            await Task.Yield();
        }

        MainMusicSource.volume = 0;
        MainMusicSource.Stop();
        MainMusicSource.clip = nextClip;
        MainMusicSource.Play();

        // --- FADE IN ---
        currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            MainMusicSource.volume = Mathf.Lerp(0, _musicVolume, currentTime / duration);
            await Task.Yield();
        }

        MainMusicSource.volume = _musicVolume;
    }

    public void SetMusicVolume(float volume)
    {
        _musicVolume = volume;
        if (MainMusicSource != null) MainMusicSource.volume = _musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSoundsVolume(float volume)
    {
        _soundsVolume = volume;
        PlayerPrefs.SetFloat("SoundsVolume", volume);
    }

    public void IncrMusicVolume(float increment)
    {
        SetMusicVolume(Mathf.Clamp01(_musicVolume + increment));
    }

    public void IncrSoundsVolume(float increment)
    {
        SetSoundsVolume(Mathf.Clamp01(_soundsVolume + increment));
    }

    public float GetMusicVolume()
    {
        return _musicVolume;
    }

    public float GetSoundsVolume()
    {
        return _soundsVolume;
    }

    public void ResetMusicVolume()
    {
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 0.5f));
    }

    public void ResetSoundsVolume()
    {
        SetSoundsVolume(PlayerPrefs.GetFloat("SoundsVolume", 0.8f));
    }
}
