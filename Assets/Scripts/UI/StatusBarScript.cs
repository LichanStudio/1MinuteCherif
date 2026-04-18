using UnityEngine;
using UnityEngine.UI;

public class StatusBarScript : MonoBehaviour
{
    public Image ForegroundImage;
    public Image BackgroundImage;
    public float LerpSpeed = 5f;
    public bool AutoMask = false;

    private float _maxValue = 100f;
    private float _currentValue = 100f;
    private float _targetFill = 1f;

    void Update()
    {
        if (ForegroundImage.fillAmount != _targetFill)
        {
            ForegroundImage.fillAmount = Mathf.Lerp(ForegroundImage.fillAmount, _targetFill, Time.deltaTime * LerpSpeed);
        }
        else if (_maxValue <= _currentValue && AutoMask)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetMaxValue(float maxValue)
    {
        _maxValue = maxValue;
        UpdateBar();
    }

    public void SetCurrentValue(float currentValue)
    {
        _currentValue = Mathf.Clamp(currentValue, 0, _maxValue);
        gameObject.SetActive(_currentValue < _maxValue);
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (ForegroundImage != null)
        {
            _targetFill = _maxValue > 0 ? _currentValue / _maxValue : 0;
        }
    }
}
