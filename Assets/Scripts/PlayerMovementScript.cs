using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementScript : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private MovementManager _movementManager;
    [SerializeField] private PlayerDataManager _playerDataManager;

    [Header("Settings")]
    [SerializeField] private Entity _playerEntity;
    [SerializeField] private Animator _playerAnimator;

    private Rigidbody2D _rigidBody;
    private Vector2 _moveInput;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.gravityScale = 0f;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        _movementManager.SetPlayer(gameObject);
    }

    void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
        /*if (_moveInput != Vector2.zero)
        {
            if (_moveInput.y > 0) _playerAnimator.Play("idle_back");
            else if (_moveInput.y < 0) _playerAnimator.Play("idle_front");
        }*/
    }

    void FixedUpdate()
    {
        _rigidBody.linearVelocity = _moveInput * _playerDataManager.GetMoveSpeed();
    }
}