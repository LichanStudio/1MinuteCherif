using System;

public static class ActionsManager
{
    public static Action OnStartSession;
    public static Action OnEndSession;
    public static Action<bool> OnSpawnCart;
    public static Action<MonsterData, int> OnDamageEnemy;
    public static Action<MonsterData> OnEntityKilled;
    public static Action<int> OnDamagePlayer;
    public static Action OnPlayerKilled;
    public static Action<Upgrade> OnTryBuyUpgrade;
    public static Action<CalculatedUpgradeClass, CalculatedUpgradeClass> OnSelectUpgrade;
    public static Action<Upgrade> OnSelectDefinitiveUpgrade;
    public static Action OnShoot;

    public static Action OnUpdateTime;
    public static Action OnUpdateRealTime;

    public static Action OnStartUpgradeSelection;

    // --------------- CHARACTER ---------------
    public static Action OnSpawnCharacter;
    public static Action<CharacterData> OnSelectCharacter;
}
