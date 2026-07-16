using System.Collections;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;

    private Animator _animator;
    private bool _isDestroyed = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.Play("idle");
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isDestroyed && collision != null && collision.CompareTag("Projectiles"))
        {
            _isDestroyed = true;
            _collider.gameObject.SetActive(false);
            StartCoroutine(AnimationManager.Instance.AnimateBreak(_animator));
        }
    }
}
