using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _buttonLabel;

    private CanvasGroup _canvasGroup;

    public void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        OnShowActionButton(false);
    }

    public void OnEnable()
    {
        ActionsManager.OnShowActionButton += OnShowActionButton;
        ActionsManager.OnTriggerDialogueZone += OnTriggerDialogueZone;
    }

    public void OnDisable()
    {
        ActionsManager.OnShowActionButton -= OnShowActionButton;
        ActionsManager.OnTriggerDialogueZone -= OnTriggerDialogueZone;
    }

    public void OnTriggerDialogueZone(bool isIn)
    {
        OnShowActionButton(isIn);
        SetButtonText("Start");
    }

    public void OnShowActionButton(bool show)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = show ? 1f : 0f;
            _canvasGroup.interactable = show;
            _canvasGroup.blocksRaycasts = show;
        }
        SetButtonText("Ok");
    }

    public void SetButtonText(string text)
    {
        if (_buttonLabel != null) _buttonLabel.text = text;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ActionsManager.OnButtonStartPressed?.Invoke();
            MovementManager.Instance.RandomTeleportPlayer();
            ActionsManager.OnStartSession?.Invoke();
        }
    }
}
