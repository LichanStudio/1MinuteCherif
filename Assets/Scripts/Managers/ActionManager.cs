using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public static class ActionsManager
{
    public static Action OnStartSession;
    public static Action OnEndSession;
    public static Action<bool> OnSpawnCart;
    public static Action<Stats> OnTryBuyUpgrade;
    public static Action<Stats, Stats> OnSelectUpgrade;
    public static Action<Stats> OnSelectDefinitiveUpgrade;
    public static Action OnShoot;

    public static Action<int, int> OnUpdateTime;
    public static Action OnUpdateRealTime;

    public static Action OnStartUpgradeSelection;

    // --------------- CHARACTER ---------------
    public static Action OnSpawnCharacter;
    public static Action<CharacterData> OnSelectCharacter;
    public static Action<PlayerScript, int> OnDamagePlayer;
    public static Action OnPlayerKilled;

    // --------------- ENEMIES ---------------
    public static Action<EnemyScript, int> OnDamageEnemy;
    public static Action<MonsterData> OnEntityKilled;
}
