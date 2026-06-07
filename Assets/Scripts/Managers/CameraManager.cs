using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private CinemachineCamera _virtualCamera;

    private float _defaultZoomLevel = 1f;
    private bool _isZoomedIn = false;

    public void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void MoveCamera(Vector3 position)
    {
        transform.position = position;
    }

    public void SetFocus(Transform target)
    {
        if (_virtualCamera == null) return;
        _virtualCamera.Follow = target;
    }

    public void SetTempZoomLevel(float zoomLevel)
    {
        if (_virtualCamera == null) return;
        if (!_isZoomedIn) _defaultZoomLevel = _virtualCamera.Lens.OrthographicSize;
        _virtualCamera.Lens.OrthographicSize = zoomLevel;
        _isZoomedIn = true;
    }

    public void ResetZoomLevel()
    {
        if (_virtualCamera == null) return;
        _virtualCamera.Lens.OrthographicSize = _defaultZoomLevel;
        _isZoomedIn = false;
    }

    public float GetZoomLevel()
    {
        if (_virtualCamera == null) return _defaultZoomLevel;
        return _virtualCamera.Lens.OrthographicSize;
    }

    public float GetDefaultZoomLevel()
    {
        return _defaultZoomLevel;
    }
}
