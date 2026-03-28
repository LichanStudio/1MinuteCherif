using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "ScriptableObjects/Data/Upgrade", order = 1)]
public class Upgrade : ScriptableObject
{
    [Serializable]
    public class UpgradeData
    {
        [SerializeField] private bool _isEnemyUpgrade;
        [SerializeField] private int _definitiveMax;
        [SerializeField] private int _definitiveMin;
        [SerializeField] private int _definitiveCost;
        [SerializeField] private int _sessionMin;
        [SerializeField] private int _sessionMax;
        [SerializeField] private UpgradeType _upgradeType;

        private int _calculatedDef;

        public bool IsEnemyUpgrade => _isEnemyUpgrade;
        public int DefinitiveMax => _definitiveMax;
        public int DefinitiveMin => _definitiveMin;
        public int DefinitiveCost => _definitiveCost;
        public int SessionMin => _sessionMin;
        public int SessionMax => _sessionMax;
        public UpgradeType UpgradeType => _upgradeType;

        public int CalculateDefValue()
        {
            _calculatedDef = UnityEngine.Random.Range(_definitiveMin, DefinitiveMax);
            return _calculatedDef;
        }

        public int GetDefValue()
        {
            return _calculatedDef;
        }

        public int CombineData(Upgrade upgrade)
        {
            _calculatedDef += upgrade.GetUpgradeData().GetDefValue();
            return _calculatedDef;
        }
    }

    public enum UpgradeType
    {
        BulletsAdd,
        BulletsMult,
        BulletsMultChance,
        MoveSpeed,
        BounceAdd,
        DamageAdd,
        HPAdd,
        HPRecovery,
        LifeSteal,
    }

    [SerializeField] private string _id;
    [SerializeField] private UpgradeData _upgradeData;

    public Upgrade Clone()
    {
        Upgrade clone = Instantiate(this);
        return clone;
    }

    public string GetId()
    {
        return _id;
    }

    public UpgradeData GetUpgradeData()
    {
        return _upgradeData;
    }
}
