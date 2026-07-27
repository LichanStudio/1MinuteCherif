using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private class AnimationState {
        public bool IsAttacking = false;
        public bool AttackEnded = false;
        public bool IsRunning = false;
        public bool IsHitted = false;
        private Coroutine _animationCoroutine;

        public void Reset() {
            IsAttacking = false;
            IsRunning = false;
            IsHitted = false;
        }

        public void StartCoroutine(MonoBehaviour monoBehaviour, IEnumerator routine) {
            Reset();
            if (_animationCoroutine != null) monoBehaviour.StopCoroutine(_animationCoroutine);
            _animationCoroutine = monoBehaviour.StartCoroutine(routine);
        }
    }

    public static AnimationManager Instance { get; private set; }

    [SerializeField] private float _attackSize = 1.5f;
    [SerializeField] private float _sizeIncreaseRate = 2f;

    private readonly string _attackAnimationName = "attack_front";
    private readonly string _hittedAnimationName = "hitted_front";
    private readonly string _runAnimationName = "run_front";
    private readonly string _breakAnimationName = "breaking";
    private readonly string _breakedAnimationName = "breaked";

    private Dictionary<Animator, AnimationState> _animationMapper = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Update()
    {
        foreach (var kvp in _animationMapper)
        {
            Animator animator = kvp.Key;
            AnimationState state = kvp.Value;
            if (animator == null) continue;
            if (state.IsAttacking)
            {
                if (state.AttackEnded)
                {
                    if (animator.gameObject.transform.localScale.x > 1f)
                    {
                        animator.gameObject.transform.localScale -= _sizeIncreaseRate * Time.deltaTime * Vector3.one;
                    }
                    else
                    {
                        animator.gameObject.transform.localScale = Vector3.one;
                    }
                }
                else
                {
                    animator.gameObject.transform.localScale += Vector3.one * Time.deltaTime;
                    if (animator.gameObject.transform.localScale.x > _attackSize) state.AttackEnded = true;
                }
            }
            else if (state.IsHitted)
            {
                // Handle hitted animation logic if needed
            }
        }
    }

    public void StartAttackAnimation(Animator animator, EntityData caster, Action actionCallback = null)
    {
        if (!_animationMapper.ContainsKey(animator)) _animationMapper.Add(animator, new());
        _animationMapper[animator].StartCoroutine(this, AttackRoutine(animator, caster, actionCallback));
    }

    public void StartHittedAnimation(Animator animator)
    {
        if (!_animationMapper.ContainsKey(animator)) _animationMapper.Add(animator, new());
        if (_animationMapper[animator].IsAttacking) return;
        _animationMapper[animator].StartCoroutine(this, HittedRoutine(animator));
    }

    private IEnumerator AttackRoutine(Animator animator, EntityData caster, Action actionCallback = null)
    {
        if (animator == null || animator.IsDestroyed()) yield break;
        _animationMapper[animator].IsAttacking = true;
        PlayAnimation(animator, _attackAnimationName);
        yield return null;
        if (animator == null) yield break;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationDuration = stateInfo.length;
        float attackDuration = GetTimeOfKeyframe(animator.runtimeAnimatorController.animationClips[0], 3);
        yield return new WaitForSeconds(attackDuration);
        if (caster != null && !caster.IsDestroyed()) actionCallback?.Invoke();
        else yield break;
        yield return new WaitForSeconds(animationDuration - attackDuration);
        if (animator == null) yield break;
        _animationMapper[animator].IsAttacking = false;
        PlayAnimation(animator, _runAnimationName);
    }

    private IEnumerator HittedRoutine(Animator animator)
    {
        if(animator == null) yield break;
        _animationMapper[animator].IsHitted = true;
        PlayAnimation(animator, _hittedAnimationName);
        yield return null;
        if(animator == null) yield break;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        if (animator == null) yield break;
        _animationMapper[animator].IsHitted = false;
        PlayAnimation(animator, _runAnimationName);
    }

    public IEnumerator AnimateBreak(Animator animator, Action actionCallback = null)
    {
        if (animator == null) yield break;
        PlayAnimation(animator, _breakAnimationName);
        yield return null;
        if (animator == null) yield break;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationDuration = stateInfo.length;
        yield return new WaitForSeconds(stateInfo.length);
        if (animator == null) yield break;
        PlayAnimation(animator, _breakedAnimationName);
        yield return null;
        actionCallback?.Invoke();
    }

    private void PlayAnimation(Animator animator, string animationName)
    {
        if (animator == null || animator.gameObject == null || animator.gameObject.IsDestroyed()) return;
        animator.gameObject.transform.localScale = Vector3.one;
        animator.Play(animationName);
    }

    public IEnumerator FlashRoutine(Material material, Color flashColor, float duration)
    {
        material.SetColor("_FlashColor", flashColor);
        material.SetFloat("_FlashAmount", 1f);

        yield return new WaitForSeconds(duration);

        material.SetFloat("_FlashAmount", 0f);
    }

    private float GetTimeOfKeyframe(AnimationClip clip, int targetKeyIndex)
    {
        UnityEditor.EditorCurveBinding[] bindings = UnityEditor.AnimationUtility.GetObjectReferenceCurveBindings(clip);

        foreach (var binding in bindings)
        {
            // Si la courbe contrôle le changement de Sprite
            if (binding.propertyName == "m_Sprite")
            {
                var keyframes = UnityEditor.AnimationUtility.GetObjectReferenceCurve(clip, binding);

                if (keyframes.Length > targetKeyIndex)
                {
                    return keyframes[targetKeyIndex - 1].time;
                }
                else if (keyframes.Length > 0)
                {
                    // Sécurité : si l'anim a moins de [targetKeyIndex] frames, on prend la toute dernière
                    return keyframes[^1].time;
                }
            }
        }

        // Valeur par défaut si rien n'est trouvé
        return 0f;
    }
}
