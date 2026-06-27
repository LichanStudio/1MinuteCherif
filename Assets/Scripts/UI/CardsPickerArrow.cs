using UnityEngine;
using UnityEngine.EventSystems;

public class CardsPickerArrow : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int _slideIncr = 1;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ActionsManager.OnSlideCards?.Invoke(_slideIncr);
        }
    }
}
