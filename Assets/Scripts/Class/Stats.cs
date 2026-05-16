using System;

[Serializable]
public class Stats
{
    public int MultishotChance = 0;
    public int MultihitChance = 0;
    public int BounceChance = 0;
    public int PiercingChance = 0;
    public int Speed = 0;
    public int HP = 0;
    public int Damage = 0;

    public static Stats operator +(Stats a, Stats b) => new()
    {
        MultishotChance = a.MultishotChance + b.MultishotChance,
        MultihitChance = a.MultihitChance + b.MultihitChance,
        BounceChance = a.BounceChance + b.BounceChance,
        PiercingChance = a.PiercingChance + b.PiercingChance,
        Speed = a.Speed + b.Speed,
        HP = a.HP + b.HP,
        Damage = a.Damage + b.Damage,
    };
    public static Stats operator *(Stats a, float b) => new()
    {
        MultishotChance = (int)(a.MultishotChance * b),
        MultihitChance = (int)(a.MultihitChance * b),
        BounceChance = (int)(a.BounceChance * b),
        PiercingChance = (int)(a.PiercingChance * b),
        Speed = (int)(a.Speed * b),
        HP = (int)(a.HP * b),
        Damage = (int)(a.Damage * b),
    };
}
