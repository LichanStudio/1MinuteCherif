using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class VolumeButtonScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] AudioManager _audioManager;
    [SerializeField] private bool _isMusic = false;
    [SerializeField] private bool _isSound = false;
    [SerializeField] private float _value = 0f;
    [SerializeField] private TextMeshProUGUI _volumeValue;

    public void OnEnable()
    {
        UpdateView();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(_audioManager != null)
        {
            if (_isMusic) _audioManager.IncrMusicVolume(_value);
            if (_isSound) _audioManager.IncrSoundsVolume(_value);
            UpdateView();
        }
    }

    public void UpdateView()
    {
        if(_volumeValue != null)
        {
            if (_isMusic) _volumeValue.text = Mathf.RoundToInt(_audioManager.GetMusicVolume() * 100).ToString() + "%";
            if (_isSound) _volumeValue.text = Mathf.RoundToInt(_audioManager.GetSoundsVolume() * 100).ToString() + "%";
        }
    }
}
