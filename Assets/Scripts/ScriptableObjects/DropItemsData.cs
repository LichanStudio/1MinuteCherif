using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/Data/DropItem", order = 1)]
public class DropItemsData : ScriptableObject
{
    public enum DropItemType
    {
        ExpOrbe,
        Shard,
    }

    [SerializeField] private string _id;
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private DropItemType _dropType = DropItemType.Shard;
    [SerializeField] private Sprite _sprite;

    public string ID => _id;
    public string ItemName => _name;
    public string Description => _description;
    public DropItemType DropType => _dropType;
    public Sprite Sprite => _sprite;
}
