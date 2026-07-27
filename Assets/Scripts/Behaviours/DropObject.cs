using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DropObject : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _pickupRange = 2f;
    [SerializeField] private float _dropRange = 1f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _speedGrow = 1f;
    [SerializeField] private DropItemsData _dropItemData;

    [Header("Game Objects")]
    [SerializeField] private SpriteRenderer _objectRenderer;
    [SerializeField] private Light2D _objectLight;
    [SerializeField] private Light2D _objectRayLight;

    private Vector3 _playerPosition = Vector3.zero;
    private bool _inRange = false;
    private int _quantity = 1;

    private void OnEnable()
    {
        ActionsManager.OnEndSession += OnEndSession;
    }

    private void OnDisable()
    {
        ActionsManager.OnEndSession -= OnEndSession;
    }

    private void Update()
    {
        if (!_inRange)
        {
            if (IsInRange(_playerPosition, transform.position, _pickupRange))
            {
                _inRange = true;
            }
        }
        else
        {
            _speed += _speedGrow * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _playerPosition, _speed * Time.deltaTime);
            if (IsInRange(_playerPosition, transform.position, _dropRange)) Drop();
        }
    }

    private void FixedUpdate()
    {
        _playerPosition = PlayerManager.Instance.PlayerObject.transform.position;
    }

    private bool IsInRange(Vector3 target, Vector3 position, float range)
    {
        Vector3 offset = target - position;
        float sqrLen = offset.sqrMagnitude;
        if (sqrLen <= range * range) return true;
        return false;
    }

    public void SetDropItemData(DropItemsData dropItemData, int quantity = 1)
    {
        _dropItemData = dropItemData;
        _quantity = quantity;
        if (_dropItemData != null)
        {
            switch(_dropItemData.DropType)
            {
                case DropItemsData.DropItemType.ExpOrbe:
                    _speedGrow = 2f;
                    if (_objectRayLight != null)
                    {
                        _objectRayLight.gameObject.SetActive(false);
                    }
                    break;
                case DropItemsData.DropItemType.Shard:
                    _speedGrow = 1f;
                    if (_objectRayLight != null)
                    {
                        _objectRayLight.gameObject.SetActive(true);
                    }
                    break;
            }
            if (_objectRenderer != null)  _objectRenderer.sprite = _dropItemData.Sprite;
        }
    }

    private void Drop()
    {
        Destroy(gameObject);
    }

    private void OnEndSession()
    {
        _inRange = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _pickupRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _dropRange);
    }
}
