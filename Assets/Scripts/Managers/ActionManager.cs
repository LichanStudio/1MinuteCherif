using System;
using System.Collections.Generic;
using UnityEngine;

public static class ActionsManager
{
    public static Action OnStartSession;
    public static Action OnEndSession;
    public static Action<bool> OnSpawnCart;
    public static Action<Entity, int> OnDamageEnemy;
    public static Action<Entity> OnEntityKilled;
    public static Action<int> OnDamagePlayer;
    public static Action OnPlayerKilled;
    public static Action<Upgrade> OnTryBuyUpgrade;
    public static Action<CalculatedUpgradeClass, CalculatedUpgradeClass> OnSelectUpgrade;
    public static Action<Upgrade> OnSelectDefinitiveUpgrade;
    public static Action OnShoot;
}
