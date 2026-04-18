using UnityEditor.U2D.Animation;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("Game Objects")]
    public GameObject PlayerPrefab;
    public GameObject PlayerObject { get; private set; }

    private CharacterData _characterData;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void OnEnable()
    {
        //_playerData.LoadPlayerData(null);
        ActionsManager.OnSelectCharacter += SetCharacter;
        ActionsManager.OnSpawnCharacter += OnSpawnCharacter;
    }

    public void OnDisable()
    {
        ActionsManager.OnSelectCharacter -= SetCharacter;
        ActionsManager.OnSpawnCharacter -= OnSpawnCharacter;
    }

    public void OnSpawnCharacter()
    {
        SpawnPlayer(Vector3.zero);
    }

    public void SpawnPlayer(Vector3 spawnPoint)
    {
        if (PlayerPrefab == null) return;
        PlayerObject = Instantiate(PlayerPrefab, spawnPoint, Quaternion.identity);
        PlayerObject.SetActive(true);
        CameraManager.Instance.SetFocus(PlayerObject.transform);
        if (_characterData != null) SetCharacter(_characterData);
    }

    public void SetCharacter(CharacterData characterData)
    {
        _characterData = characterData;
        if (PlayerPrefab != null && PlayerPrefab != null && PlayerPrefab.TryGetComponent(out PlayerScript playerScript))
        {
            if (characterData != null && characterData.Animator != null) playerScript.SetAnimatorController(characterData.Animator);
            else playerScript.SetAnimatorController(null);
        }
    }
}