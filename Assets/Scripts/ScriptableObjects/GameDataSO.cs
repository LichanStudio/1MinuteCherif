using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/Data/GameData", order = 1)]
public class GameData : ScriptableObject
{
    [SerializeField] private int _secondsToPlay = 60;

    public int SecondsToPlay => _secondsToPlay;
}
