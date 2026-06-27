using UnityEngine;
using UnityEngine.EventSystems;

public class StartButton : MonoBehaviour, IPointerClickHandler
{
    private float _avoidZone = 50f;

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
