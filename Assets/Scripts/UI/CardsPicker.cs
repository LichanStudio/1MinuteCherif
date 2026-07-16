using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class CardsPicker : MonoBehaviour
{
    [SerializeField] private float _cardsWidth;
    [SerializeField] private float _cardsHeight;
    [SerializeField] private float _minScale;
    [SerializeField] private float _maxScale;
    [SerializeField] private float _minAlpha = 0.2f;
    [SerializeField] private AnimationCurve _alphaCurve;
    [SerializeField] private List<CardMap> _cardsPool;
    [SerializeField] private List<Transform> _cardsPositions;

    private int _startIndex = 0;
    private string _selectedCard;

    public void OnEnable()
    {
        ActionsManager.OnSlideCards += OnSlideCards;
        ActionsManager.OnButtonStartPressed += OnButtonStartPressed;
        StartSelection(0);
    }

    public void OnDisable()
    {
        ActionsManager.OnSlideCards -= OnSlideCards;
        ActionsManager.OnButtonStartPressed -= OnButtonStartPressed;
    }

    public void StartSelection(int move = 0)
    {
        MoveCards(move);
        MapsManager mapsManager = MapsManager.Instance;
        int midCardIndex = 0;
        if (_cardsPool.Count > 0)
        {
            midCardIndex = _cardsPool.Count / 2;
        }
        for (int i = 0; i < _cardsPool.Count; i++)
        {
            CardMap card = _cardsPool[i];
            card.SetPosition(i, _cardsPositions);

            int actuelIndex = _startIndex + i - midCardIndex;
            float alpha = _alphaCurve.Evaluate(i / (float)(_cardsPool.Count-1));
            int zIndex = (int)(alpha * 6f);

            if (i == 0 || i == _cardsPool.Count - 1) alpha = 0f;
            else if (alpha < _minAlpha) alpha = _minAlpha;

            card.SetZIndex(zIndex);
            if (card.SetIndex(actuelIndex))
            {
                card.Fade(alpha);
                card.SetMap(mapsManager.GetMapByIndex(actuelIndex));
                if (_startIndex == actuelIndex) _selectedCard = card.GetMapId();
            }
            else
            {
                card.SetMap(null);
            }
        }
    }

    private void MoveCards(int moveValue)
    {
        int totalCartes = _cardsPool.Count;
        if (totalCartes <= 1 || moveValue == 0) return;

        CardMap[] tableauTemporaire = new CardMap[totalCartes];

        for (int i = 0; i < totalCartes; i++)
        {
            int nouvelIndex = (i + moveValue) % totalCartes;
            if (nouvelIndex < 0) nouvelIndex += totalCartes;
            tableauTemporaire[nouvelIndex] = _cardsPool[i];
        }

        _cardsPool = new List<CardMap>(tableauTemporaire);
    }

    public void Left()
    {
        if (_startIndex > 0)
        {
            _startIndex--;
            StartSelection(1);
        }
    }

    public void Right()
    {
        if (_startIndex < 99)
        {
            _startIndex++;
            StartSelection(-1);
        }
    }

    public void OnSlideCards(int slideValue)
    {
        if (_startIndex + slideValue > MapsManager.Instance.GetMapsCount() - 1) return;
        if (_startIndex + slideValue < 0) return;
        _startIndex += slideValue;
        StartSelection(-slideValue);
    }

    public void OnButtonStartPressed()
    {
        if (string.IsNullOrEmpty(_selectedCard)) return;
        Debug.Log($"start with {_selectedCard}");
        ActionsManager.OnSelectMap?.Invoke(_selectedCard);
    }
}
