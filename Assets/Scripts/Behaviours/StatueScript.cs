using System.Collections;
using TMPro;
using UnityEngine;

public class StatueScript : MonoBehaviour
{
    [SerializeField] private AnimationCurve _dialogueAlphaCurve;
    [SerializeField] private AnimationCurve _dialogueSizeCurve;
    [SerializeField] private GameObject _dialogueFrame;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private GameObject _cardsContainer;

    [SerializeField] private float _dialogueSize = 0.2f;
    [SerializeField] private float _dialogueSpeed = 20f;
    [SerializeField] private float _timeAutoSkip = 1f;

    private bool _isInDialogueZone = false;
    private bool _openingDialogue = false;
    private bool _animateDialogue = false;
    private float _dialogueAnimationTime = 0f;
    private float _dialogueTextTime = 0f;
    private CanvasGroup _dialogueCanvasGroup;
    private Coroutine _autoSkipCoroutine;

    private string _dialogue = "Where do you want to go ?";

    private void OnEnable()
    {
        ActionsManager.OnTriggerDialogueZone += OnTriggerDialogueZone;
    }

    private void OnDisable()
    {
        ActionsManager.OnTriggerDialogueZone -= OnTriggerDialogueZone;
    }

    private void Update()
    {
        if (_openingDialogue)
        {
            _dialogueAnimationTime += Time.deltaTime;
            float alpha = _dialogueAlphaCurve.Evaluate(_dialogueAnimationTime);
            float size = _dialogueSizeCurve.Evaluate(_dialogueAnimationTime) * _dialogueSize;
            if (alpha >= 1f)
            {
                _openingDialogue = false;
                _dialogueAnimationTime = 0f;
                alpha = 1f;
                size = _dialogueSize;
            }
            _dialogueFrame.transform.localScale = new Vector3(size, size, 1f);
            if (_dialogueCanvasGroup == null && _dialogueFrame.TryGetComponent(out CanvasGroup canvasGroup)) _dialogueCanvasGroup = canvasGroup;
            if (_dialogueCanvasGroup != null) _dialogueCanvasGroup.alpha = alpha;
        }
        if (_animateDialogue)
        {
            _dialogueTextTime += Time.deltaTime;
            int charactersToShow = Mathf.Min((int)(_dialogueTextTime * _dialogueSpeed), _dialogue.Length);
            _dialogueText.text = _dialogue[..charactersToShow];
            if (charactersToShow >= _dialogue.Length)
            {
                _animateDialogue = false;
                _dialogueTextTime = 0f;
                _autoSkipCoroutine = StartCoroutine(AutoSkipCouroutine());
            }
        }
    }

    private void OnTriggerDialogueZone(bool trigger)
    {
        _isInDialogueZone = trigger;
        _openingDialogue = trigger;
        _animateDialogue = trigger;
        if (trigger) OnTriggerIn();
        else OnTriggerOut();
    }

    private void OnTriggerOut()
    {
        _isInDialogueZone = false;
        _openingDialogue = false;
        _animateDialogue = false;
        _dialogueAnimationTime = 0f;
        _dialogueTextTime = 0f;
        _dialogueFrame.transform.localScale = new Vector3(0f, 0f, 1f);
        _dialogueText.text = string.Empty;
        if (_dialogueCanvasGroup != null) _dialogueCanvasGroup.alpha = 0f;
        _dialogueFrame.gameObject.SetActive(false);
        _cardsContainer.gameObject.SetActive(false);
        if (_autoSkipCoroutine != null)
        {
            StopCoroutine(_autoSkipCoroutine);
            _autoSkipCoroutine = null;
        }
    }

    private void OnTriggerIn()
    {
        _isInDialogueZone = true;
        _openingDialogue = true;
        _animateDialogue = true;
        _dialogueFrame.gameObject.SetActive(true);
    }

    private IEnumerator AutoSkipCouroutine()
    {
        yield return new WaitForSeconds(_timeAutoSkip);
        if (_isInDialogueZone)
        {
            if (_cardsContainer != null) _cardsContainer.SetActive(true);
            if (_dialogueFrame != null) _dialogueFrame.SetActive(false);
        }
    }
}
