using TMPro;
using UnityEngine;

public class UpgradeLineUIScript : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI _upgradeLabel;
    [SerializeField] public TextMeshProUGUI _upgradeValue;

    public void SetUpgradeLine(string label, string value)
    {
        _upgradeLabel.text = label;
        _upgradeValue.text = value;
    }
}
