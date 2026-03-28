using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightScript : MonoBehaviour
{
    [Header("Manager")]
    [SerializeField] private GameManager _gameManager;

    [Header("Settings")]
    [SerializeField] private Light2D _globalLight;
    [SerializeField] private float _lerpSpeed = 5f;

    private float _targetIntensity;
    private float _intensity;

    public void Start()
    {
        _intensity = 1f;
        _targetIntensity = 1f;
        if (_globalLight != null) _intensity = _globalLight.intensity;
        if (_gameManager != null) _gameManager.SetGlobalLightScript(this);
    }

    public void Update()
    {
        if (_globalLight == null) return;
        if (Mathf.Approximately(_intensity, _targetIntensity)) _intensity = _targetIntensity;
        else _intensity = Mathf.Lerp(_intensity, _targetIntensity, Time.deltaTime * _lerpSpeed);
        _globalLight.intensity = _intensity;
    }

    public void SetGlobalLightIntensity(float intensity)
    {
        _targetIntensity = intensity;
    }
}
