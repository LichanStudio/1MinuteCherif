using UnityEngine;

public class MapTextureScript : MonoBehaviour
{
    private SpriteRenderer _mapRenderer;

    private void OnEnable()
    {
        ActionsManager.OnSelectMap += OnSelectMap;
    }

    private void OnDisable()
    {
        ActionsManager.OnSelectMap += OnSelectMap;
    }

    private void Start()
    {
        _mapRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnSelectMap(string mapId)
    {
        MapData mapData = MapsManager.Instance.SetActualMap(mapId);
        if (mapData == null) return;
        _mapRenderer.material = mapData.MapMaterial;
    }
}
