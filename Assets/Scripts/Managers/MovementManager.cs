using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CreateAssetMenu(fileName = "MovementManager", menuName = "ScriptableObjects/Managers/Movement", order = 1)]
public class MovementManager : ScriptableObject
{
    private GameObject _player;

    public void SetPlayer(GameObject player)
    {
        _player = player;
    }

    public GameObject GetPlayer()
    {
        return _player;
    }

    public Vector2 MoveTowardPlayer(GameObject objectToMove, float stoppingDistance = 0f)
    {
        if (_player == null || objectToMove == null) return Vector2.zero;
        float distance = Vector2.Distance(objectToMove.transform.position, _player.transform.position);
        if (distance > stoppingDistance) return ((Vector2)_player.transform.position - (Vector2)objectToMove.transform.position).normalized;
        return Vector2.zero;
    }

    public void TeleportPlayer(Vector2 newPosition)
    {
        if (_player == null) return;
        _player.transform.position = newPosition;
    }
}
