using UnityEngine;

public class EntityScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected float _hittedDuration = 0.1f;
    [SerializeField] protected Color _hittedFlash = Color.white;

    [Header("Game Objects")]
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected Animator _animator;

    private Coroutine _flashCoroutine;
    protected Collider2D[] _colliders2D;

    public virtual void Awake()
    {
        _colliders2D = GetComponents<Collider2D>();
    }

    protected void OnHitted()
    {
        if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(AnimationManager.Instance.FlashRoutine(_spriteRenderer.material, _hittedFlash, _hittedDuration));
    }
}
