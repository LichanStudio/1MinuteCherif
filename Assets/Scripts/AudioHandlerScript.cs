using System.Collections.Generic;
using UnityEngine;

public class AudioHandlerScript : MonoBehaviour
{
    [Header("Managers")]
    public AudioManager AudioMng;
    public GameManager GameMng;

    [Header("AudioSources")]
    public AudioSource MainMusicSource;
    public List<AudioSource> SoundsSources;

    public void OnEnable()
    {
        if (MainMusicSource != null) MainMusicSource.Stop();
        if (AudioMng != null)
        {
            if (MainMusicSource != null) AudioMng.MainMusicSource = MainMusicSource;
            if (SoundsSources != null) AudioMng.SoundsSources = SoundsSources;
        }
        ActionsManager.OnDamageEnemy += OnDamageEnemy;
        ActionsManager.OnDamagePlayer += OnDamagePlayer;
        ActionsManager.OnStartSession += OnSessionStart;
        ActionsManager.OnEndSession += OnSessionEnd;
        ActionsManager.OnSpawnProjectile += OnShoot;
        ActionsManager.OnSelectDefinitiveUpgrade += OnBuyUpgrade;
    }

    public void OnDisable()
    {
        ActionsManager.OnDamageEnemy -= OnDamageEnemy;
        ActionsManager.OnDamagePlayer -= OnDamagePlayer;
        ActionsManager.OnStartSession -= OnSessionStart;
        ActionsManager.OnEndSession -= OnSessionEnd;
        ActionsManager.OnSpawnProjectile -= OnShoot;
        ActionsManager.OnSelectDefinitiveUpgrade -= OnBuyUpgrade;
    }

    private void OnDamageEnemy(EnemyScript entity, int damage)
    {
        OnHit();
    }

    private void OnDamagePlayer(PlayerScript playerScript, int damage)
    {
        OnHit();
    }

    private void OnHit()
    {
        if (AudioMng == null) return;
        AudioMng.PlayHitSound();
    }

    private void OnShoot()
    {
        if (AudioMng == null) return;
        AudioMng.PlayShootSound();
    }

    private void OnBuyUpgrade(Stats stats)
    {
        if (AudioMng == null) return;
        AudioMng.PlayBuySound();
    }

    private void OnSessionStart()
    {
        if (AudioMng == null) return;
        AudioMng.PlayMusicBattle();
    }

    private void OnSessionEnd()
    {
        if (AudioMng == null) return;
        AudioMng.PlayMusicSaloon();
    }
}