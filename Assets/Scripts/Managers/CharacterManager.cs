using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

[DefaultExecutionOrder(100)]
public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    [Header("Registries")]
    public CharactersRegistry _charactersRegistry;

    public CharacterData SelectedCharacter { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void OnEnable()
    {
        SelectedCharacter = GetUnlockedCharacters().FirstOrDefault();
        SelectCharacter(SelectedCharacter);
        ActionsManager.OnSpawnCharacter?.Invoke();
        ActionsManager.OnSelectUpgrade += OnSelectUpgrade;
        ActionsManager.OnStartSession += OnStartSession;
    }

    public void OnDisable()
    {
        ActionsManager.OnSelectUpgrade -= OnSelectUpgrade;
        ActionsManager.OnStartSession -= OnStartSession;
    }

    public List<CharacterData> GetUnlockedCharacters()
        => _charactersRegistry.characters.Where(c => c.IsUnlockedByDefault).ToList();

    public void SelectCharacter(CharacterData character)
    {
        SelectedCharacter = character;
        ActionsManager.OnSelectCharacter?.Invoke(character);
    }

    public void OnStartSession()
    {
        SelectedCharacter.ResetAditionnalStats();
    }

    public void OnSelectUpgrade(Stats characterStats, Stats enemiesStats)
    {
        SelectedCharacter.AddAditionnalStats(characterStats);
    }
}
