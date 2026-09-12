using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _EscUI;

    private bool _isEsc = false;
    private bool _wasPaused = false;

    public void Awake()
    {
        _isEsc = false;
    }

    public void OnEnable()
    {
        if (_EscUI != null) _EscUI.SetActive(_isEsc);
    }

    public void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!_isEsc) _wasPaused = GameManager.Instance.IsGamePaused();
            OnPressEsc();
        }
    }

    void OnClick(InputValue value)
    {
        if (!value.isPressed) return;
        ActionsManager.OnClick?.Invoke();
    }

    public void OnPressEsc()
    {
        _isEsc = !_isEsc;
        if (_EscUI != null) _EscUI.SetActive(_isEsc);
        if (!_wasPaused) GameManager.Instance.TogglePause(_isEsc);
    }
}
