using System;
using UnityEngine;

public abstract class EntityScript<T> : MonoBehaviour, IEntityScript where T : EntityData
{
    [Serializable]
    public class EntitySkill
    {
        [SerializeField] private SkillData _skillData;
        [SerializeField] private GameObject _skillPrefab;
    }

    [Header("Entity Settings")]
    [SerializeField] protected T _entityData;
    [SerializeField] protected float _hittedDuration = 0.1f;
    [SerializeField] protected Color _hittedFlash = Color.white;

    [Header("Entity Game Objects")]
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected SpriteRenderer _shadowRenderer;
    [SerializeField] protected Animator _animator;

    private Coroutine _flashCoroutine;
    protected Collider2D[] _colliders2D;
    protected bool _isPlayer = false;

    public virtual void Awake()
    {
        _colliders2D = GetComponents<Collider2D>();
    }

    public bool IsPlayer => _isPlayer;
    public T Data => _entityData;
    EntityData IEntityScript.BaseData => _entityData;

    public abstract void TakeDamage(int damage, GameObject dmgLabel = null);

    protected void OnHitted()
    {
        if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(AnimationManager.Instance.FlashRoutine(_spriteRenderer.material, _hittedFlash, _hittedDuration));
    }

    public EntityData GetEntityData()
    {
        return _entityData;
    }
}
