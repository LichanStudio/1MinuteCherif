using System.Collections;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Transform DialoguePoint = null;
    [SerializeField] private float _targetZoomLevel = 10f;
    [SerializeField] private int _zoomTics = 50;

    private bool isIn = false;
    private float _zoomLevel = 0f;
    private Coroutine _zoomCoroutine;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (_zoomCoroutine != null) StopCoroutine(_zoomCoroutine);
        _zoomCoroutine = StartCoroutine(SmoothZoom(_targetZoomLevel));
        isIn = true;
        ReloadFocusPoint();
        ActionsManager.OnTriggerDialogueZone?.Invoke(true);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (_zoomCoroutine != null) StopCoroutine(_zoomCoroutine);
        _zoomCoroutine = StartCoroutine(SmoothZoom(CameraManager.Instance.GetDefaultZoomLevel()));
        isIn = false;
        ReloadFocusPoint();
        ActionsManager.OnTriggerDialogueZone?.Invoke(false);
    }

    public void ReloadFocusPoint()
    {
        if (DialoguePoint == null) return;
        Transform focus;
        if (isIn) focus = DialoguePoint;
        else focus = PlayerManager.Instance.PlayerObject.transform;
        CameraManager.Instance.SetFocus(focus);
    }

    public void SetZoomLevel(float zoomLevel = 0f)
    {
        if (zoomLevel != 0f) CameraManager.Instance.SetTempZoomLevel(zoomLevel);
        else CameraManager.Instance.ResetZoomLevel();
    }

    private IEnumerator SmoothZoom(float targetZoomLevel)
    {
        _zoomLevel = CameraManager.Instance.GetZoomLevel();
        CameraManager camManager = CameraManager.Instance;
        float tic = (targetZoomLevel - _zoomLevel) / (float)_zoomTics;
        int tics = 0;
        while (tics < _zoomTics)
        {
            yield return new WaitForSeconds(0.01f);
            camManager.SetTempZoomLevel(_zoomLevel);
            _zoomLevel += tic;
            tics++;
        }
    }
}
