using TMPro;
using UnityEngine;

public class StatusFrameScript : MonoBehaviour
{
    public TextMeshProUGUI TimeLabel;

    public void OnEnable()
    {
        ActionsManager.OnUpdateTime += UpdateTime;
    }

    public void OnDisable()
    {
        ActionsManager.OnUpdateTime -= UpdateTime;
    }

    private void UpdateTime()
    {
        if (TimeLabel != null) TimeLabel.text = TimeManager.Instance.GetSecondsLeft().ToString();
    }
}
