using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
public class CardMap : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private AnimationCurve _animationCurve;
    [SerializeField] private float _animationDuration = 1.0f;
    [SerializeField] private float _minScale = 0.04f;
    [SerializeField] private float _maxScale = 0.08f;
    [SerializeField] private Color _defaultColor = Color.gray;

    [Header("Game Objects")]
    [SerializeField] private Image _cardRenderer;
    [SerializeField] private TextMeshProUGUI _mapLabel;
    [SerializeField] private TextMeshProUGUI _mapNumber;

    private MapData _mapData;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private float _alpha = 0f;
    private int? _index = null;
    private bool _active = true;
    private float _animationTime = 0f;
    private Vector2 _initialPos = Vector2.zero;
    private Vector2 _targetPosition = Vector2.zero;
    private float _initialAlpha = 0f;
    private float _targetAlpha = 0f;
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
        float timePercent = 0f;
        if (_animationDuration > 0f) timePercent = _animationTime / _animationDuration;
        float progressionCourbe = _animationCurve.Evaluate(timePercent);

        transform.position = Vector3.LerpUnclamped(_initialPos, _targetPosition, progressionCourbe);
        _alpha = Mathf.Lerp(_initialAlpha, _targetAlpha, progressionCourbe);

        if (timePercent >= 1f)
        {
            transform.position = _targetPosition;
            _alpha = _targetAlpha;
            _isMoving = false;
            _animationTime = 0f;
        }

        _canvasGroup.alpha = _alpha;
        transform.localScale = Vector3.one * Mathf.Lerp(_minScale, _maxScale, _alpha);
    }

    public bool SetIndex(int index)
    {
        _index = index;
        if (_mapNumber != null) _mapNumber.text = (index + 1).ToString();
        if (_index < 0 || MapsManager.Instance == null || _index > MapsManager.Instance.GetMapsCount() - 1)
        {
            Fade(0f);
            return false;
        }
        return true;
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
        StartAnimate();
    }

    public void Fade(float targetAlpha)
    {
        if (_canvasGroup != null) _alpha = _canvasGroup.alpha;
        else _alpha = 0f;
        _initialAlpha = _alpha;
        _targetAlpha = targetAlpha;
        _active = targetAlpha > 0f;
        StartAnimate();
    }

    public bool IsActive()
    {
        return _active;
    }

    public string GetMapId()
    {
        if(!_mapData) return string.Empty;
        return _mapData.Id;
    }

    public void SetMap(MapData mapData)
    {
        _mapData = mapData;
        if (mapData != null && !string.IsNullOrEmpty(mapData.MapName)) _mapLabel.text = mapData.MapName;
        else _mapLabel.text = string.Empty;
        if (_cardRenderer != null)
        {
            Color newColor = _defaultColor;
            if (mapData != null && mapData.MainColor != null) newColor = mapData.MainColor;
            newColor.a = 1f;
            _cardRenderer.color = newColor;
        }
    }

    public void StartAnimate()
    {
        _isMoving = true;
        _animationTime = 0f;
    }
}
