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

    private void UpdateTime(int timePassed, int totalTime)
    {
        if (TimeLabel != null) TimeLabel.text = (totalTime - timePassed).ToString();
    }
}
