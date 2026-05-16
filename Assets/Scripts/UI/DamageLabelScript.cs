using TMPro;
using UnityEngine;

public class DamageLabelScript : MonoBehaviour
{
    [Header("Components")]
    public TextMeshPro Label;

    [Header("Settings")]
    public float FadingSpeed = 2f;
    public float MovingSpeed = 2f;
    public AnimationCurve FadeIn;
    public AnimationCurve FadeOut;
    public AnimationCurve MoveAnimation;


    private bool _fadingIn = false;
    private bool _fadingOut = false;
    private bool _moving = false;
    private float _fadingTime = 0f;
    private float _moveTime = 0f;

    public void Start()
    {
        if (Label == null) Label = GetComponentInChildren<TextMeshPro>();
    }

    public void SetDamage(int damage)
    {
        if (Label != null) Label.text = damage.ToString();
    }

    public void OnEnable()
    {
        _fadingIn = true;
        _fadingOut = false;
        _moving = true;
        _fadingTime = 0f;
        _moveTime = 0f;
    }

    public void OnDisable()
    {
        _fadingIn = false;
        _fadingOut = false;
        _moving = false;
        _fadingTime = 0f;
        _moveTime = 0f;
    }

    public void OnDestroy()
    {
        Debug.Log("Damage label destroyed");
    }

    public void Update()
    {
        if (_fadingIn)
        {
            _fadingTime += Time.deltaTime * FadingSpeed;
            float alpha = FadeIn.Evaluate(_fadingTime);
            SetAlpha(alpha);
            if (alpha >= 1f)
            {
                _fadingIn = false;
                _fadingOut = true;
            }
        }
        else if (_fadingOut)
        {
            _fadingTime -= Time.deltaTime * FadingSpeed;
            float alpha = FadeOut.Evaluate(_fadingTime);
            SetAlpha(alpha);
            if (alpha <= 0f)
            {
                DamageManager.Instance.GetBackLabelInPool(gameObject);
            }
        }
        if (_moving)
        {
            _moveTime += Time.deltaTime * MovingSpeed;
            float moveY = MoveAnimation.Evaluate(_moveTime);
            transform.localPosition = new(0f, moveY, 0f);
            if (_moveTime >= 1f) _moving = false;
        }
    }

    private void SetAlpha(float alpha)
    {
        if (Label != null)
        {
            Color color = Label.color;
            color.a = alpha;
            Label.color = color;
        }
    }
}
