using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private CinemachineCamera _virtualCamera;

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
}
