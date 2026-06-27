using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Animator _playerAnimator;

    [Header("Game Objects")]
    [SerializeField] private Material _mapNoise;
    [SerializeField] private Material _mapRender;

    private Rigidbody2D _rigidBody;
    private Vector2 _moveInput;
    private MovementManager.MovementType _currentMovementType = MovementManager.MovementType.Idle;
    private MovementManager.MovementType _lastMovementType = MovementManager.MovementType.Idle;
    private MovementManager.MovementDirection _currentMovementDirection = MovementManager.MovementDirection.Down;
    private MovementManager.MovementDirection _lastMovementDirection = MovementManager.MovementDirection.Down;

    public void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.gravityScale = 0f;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
        _currentMovementType = _moveInput == Vector2.zero ? MovementManager.MovementType.Idle : MovementManager.MovementType.Run;
        if( _moveInput.x > 0) _currentMovementDirection = MovementManager.MovementDirection.Right;
        else if (_moveInput.x < 0) _currentMovementDirection = MovementManager.MovementDirection.Left;
        else if (_moveInput.y > 0) _currentMovementDirection = MovementManager.MovementDirection.Up;
        else if (_moveInput.y < 0) _currentMovementDirection = MovementManager.MovementDirection.Down;

        if (_currentMovementType == _lastMovementType && _currentMovementDirection == _lastMovementDirection) return;

        string startAnimation = _currentMovementType == MovementManager.MovementType.Run ? "run_" : "idle_";
        switch (_currentMovementDirection)
        {
            case MovementManager.MovementDirection.Up: _playerAnimator.Play(startAnimation + "back"); break;
            case MovementManager.MovementDirection.Down: _playerAnimator.Play(startAnimation + "front"); break;
            case MovementManager.MovementDirection.Left: _playerAnimator.Play(startAnimation + "left"); break;
            case MovementManager.MovementDirection.Right: _playerAnimator.Play(startAnimation + "right"); break;
        }

        _lastMovementDirection = _currentMovementDirection;
        _lastMovementType = _currentMovementType;
    }

    public void FixedUpdate()
    {
        int speed = CharacterManager.Instance.SelectedCharacter.GetTotalStats().Speed;
        _rigidBody.linearVelocity = _moveInput * (float)(speed / 10f);
    }

    public void Update()
    {
        _mapNoise.SetVector("_PlayerPosition", transform.position);
        MapData mapData = MapsManager.Instance.GetActualMap();
        if (mapData == null || mapData.MapMaterial == null) return;
        mapData.MapMaterial.SetVector("_PlayerPosition", transform.position);
    }
}