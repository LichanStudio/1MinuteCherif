using TMPro;
using UnityEngine;

public class DefinitiveUpgradeScript : MonoBehaviour
{
    [SerializeField] private UpgradeLineUIScript _upgradeLine;
    [SerializeField] private TextMeshProUGUI _costValue;

    private bool _isIn = false;
    private Upgrade _upgrade = null;

    public void OnEnable()
    {
        _isIn = false;
    }

    public void OnDisable()
    {
        _isIn = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision == null) return;
        if (collision.isTrigger && collision.gameObject.CompareTag("Player"))
        {
            _isIn = true;
        }
        if (_isIn && collision.isTrigger && collision.gameObject.CompareTag("Projectiles"))
        {
            ActionsManager.OnTryBuyUpgrade?.Invoke(_upgrade);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.isTrigger && collision.gameObject.CompareTag("Player"))
        {
            _isIn = false;
        }
    }

    public void SetUpgrade(Upgrade upgrade)
    {
        string label = "";
        string unit = "";
        _upgrade = upgrade;
        switch (upgrade.GetUpgradeData().UpgradeType)
        {
            case Upgrade.UpgradeType.DamageAdd:label = "Damage"; break;
            case Upgrade.UpgradeType.BulletsAdd:label = "Bullets"; break;
            case Upgrade.UpgradeType.BounceAdd:label = "Bounce"; break;
            case Upgrade.UpgradeType.BulletsMult:label = "Split bullets"; break;
            case Upgrade.UpgradeType.BulletsMultChance:label = "Chance of split"; unit = "%"; break;
            case Upgrade.UpgradeType.HPAdd:label = "Increase HP"; break;
            case Upgrade.UpgradeType.HPRecovery:label = "HP Recovery"; unit = "%"; break;
            case Upgrade.UpgradeType.LifeSteal:label = "Life steal"; unit = "%"; break;
            case Upgrade.UpgradeType.MoveSpeed:label = "Move speed"; unit = "%"; break;
        }
        _upgradeLine.SetUpgradeLine(label, upgrade.GetUpgradeData().GetDefValue().ToString() + unit);
        _costValue.text = upgrade.GetUpgradeData().DefinitiveCost.ToString();
    }
}
