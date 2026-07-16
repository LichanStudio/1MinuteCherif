using UnityEngine;

public class DropObject : MonoBehaviour
{
    [SerializeField] private float _pickupRange = 2f;
    [SerializeField] private float _dropRange = 1f;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _speedGrow = 1f;

    private Vector3 _playerPosition = Vector3.zero;
    private bool _inRange = false;

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
