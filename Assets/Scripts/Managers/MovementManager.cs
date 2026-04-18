using UnityEngine;

[DefaultExecutionOrder(-100)]
public class MovementManager : MonoBehaviour
{
    public enum MovementDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum MovementType
    {
        Idle,
        Run
    }

    public static MovementManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Vector2 MoveTowardPlayer(GameObject objectToMove, float stoppingDistance = 0f)
    {
        if (PlayerManager.Instance == null || objectToMove == null) return Vector2.zero;
        float distance = Vector2.Distance(objectToMove.transform.position, PlayerManager.Instance.PlayerObject.transform.position);
        if (distance > stoppingDistance) return (Vector2)(PlayerManager.Instance.PlayerObject.transform.position - objectToMove.transform.position).normalized;
        return Vector2.zero;
    }

    public void TeleportPlayer(Vector2 newPosition)
    {
        if (PlayerManager.Instance == null || PlayerManager.Instance.PlayerObject == null) return;
        PlayerManager.Instance.PlayerObject.transform.position = newPosition;
    }
}
