using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stats
{
    public int MultishotChance = 0;
    public int Speed = 0;
    public int HP = 0;
    public int Damage = 0;

    public static Stats operator +(Stats a, Stats b) => new()
    {
        MultishotChance = a.MultishotChance + b.MultishotChance,
        Speed = a.Speed + b.Speed,
        HP = a.HP + b.HP,
        Damage = a.Damage + b.Damage,
    };
}
