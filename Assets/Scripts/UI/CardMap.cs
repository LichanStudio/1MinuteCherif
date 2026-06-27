using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
public class CardMap : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private AnimationCurve _animationCurve;
    [SerializeField] private float _animationDuration = 1.0f;
    [SerializeField] private float _minScale = 0.04f;
    [SerializeField] private float _maxScale = 0.08f;

    [Header("Game Objects")]
    [SerializeField] private TextMeshProUGUI _mapLabel;

    private string _mapId;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private float _alpha = 1f;
    private int? _index = null;
    private bool _active = true;
    private float _animationTime = 0f;
    private Vector2 _initialPos = Vector2.zero;
    private Vector2 _targetPosition = Vector2.zero;
    private float _initialAlpha = 0f;
    private float _targetAlpha = 0.5f;
    private bool _isMoving = false;

    public void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Update()
    {
        if (!_isMoving) return;

        _animationTime += Time.deltaTime;
        float pourcentageTemps = _animationTime / _animationDuration;
        float progressionCourbe = _animationCurve.Evaluate(pourcentageTemps);

        transform.position = Vector3.LerpUnclamped(_initialPos, _targetPosition, progressionCourbe);
        _alpha = Mathf.Lerp(_initialAlpha, _targetAlpha, progressionCourbe);

        if (pourcentageTemps >= 1f)
        {
            transform.position = _targetPosition;
            _alpha = _targetAlpha;
            _isMoving = false;
            _animationTime = 0f;
        }

        _canvasGroup.alpha = _alpha;
        transform.localScale = Vector3.one * Mathf.Lerp(_minScale, _maxScale, _alpha);
    }

    public void SetIndex(int index)
    {
        bool change = _index != null && _index != index;
        _index = index;
        if (_index < 0 || MapsManager.Instance == null || _index > MapsManager.Instance.GetMapsCount() - 1)
        {
            _alpha = 0;
            Fade(0f);
            _isMoving = true;
        }
    }

    public int? GetIndex()
    {
        return _index;
    }

    public void SetZIndex(int zIndex)
    {
        if (_canvas == null) _canvas = GetComponent<Canvas>();
        if (_canvas != null) _canvas.sortingOrder = zIndex;
    }

    public void SetPosition(int index, List<Transform> positions)
    {
        _targetPosition = positions[index].position;
        _initialPos = transform.position;
        _isMoving = true;
    }

    public void Fade(float targetAlpha)
    {
        if (_canvasGroup == null)
        {
            _alpha = targetAlpha;
            return;
        }
        _alpha = _canvasGroup.alpha;
        _initialAlpha = _alpha;
        _targetAlpha = targetAlpha;
        _active = targetAlpha > 0f;
        _isMoving = true;
    }

    public bool IsActive()
    {
        return _active;
    }

    public string GetMapId()
    {
        return _mapId;
    }

    public void SetMapId(string id)
    {
        _mapId = id;
        if (!string.IsNullOrEmpty(id)) _mapLabel.text = id;
        else _mapLabel.text = string.Empty;
    }
}
