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

    private float _teleportZone = 1000f;
    private float _avoidZone = 100f;

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

    public void RandomTeleportPlayer()
    {
        int coteAuHasard = UnityEngine.Random.Range(0, 4);
        Vector2 positionSpawn = Vector2.zero;

        switch (coteAuHasard)
        {
            case 0: // Bande GAUCHE
                positionSpawn.x = UnityEngine.Random.Range(-_teleportZone, -_avoidZone);
                positionSpawn.y = UnityEngine.Random.Range(_teleportZone, _teleportZone);
                break;

            case 1: // Bande DROITE
                positionSpawn.x = UnityEngine.Random.Range(_avoidZone, _teleportZone);
                positionSpawn.y = UnityEngine.Random.Range(_teleportZone, _teleportZone);
                break;

            case 2: // Bande INFÉRIEURE (on restreint le X pour ne pas doubler les coins)
                positionSpawn.x = UnityEngine.Random.Range(-_avoidZone, _avoidZone);
                positionSpawn.y = UnityEngine.Random.Range(_teleportZone, -_avoidZone);
                break;

            case 3: // Bande SUPÉRIEURE (on restreint le X pour ne pas doubler les coins)
                positionSpawn.x = UnityEngine.Random.Range(-_avoidZone, _avoidZone);
                positionSpawn.y = UnityEngine.Random.Range(_avoidZone, _teleportZone);
                break;
        }

        TeleportPlayer(positionSpawn);
    }
}
