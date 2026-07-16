using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTerrainDetector : MonoBehaviour
{
    [Header("Références")]
    public CustomRenderTexture mapRenderTexture;
    public Transform mapRendererTransform;
    public Transform playerTransform;
    public SpriteRenderer testRenderer;

    [Header("Paramètres")]
    public float testX = 1f;
    public float testY = 1f;
    public float zoomFactor = 2f;
    public float detectionDelay = 0.2f;

    [Header("Debug")]
    public float currentNoiseValue;
    public TileType _terrainValue = TileType.None;
    public Color testColor;
    public int xDebug = 1024;
    public int yDebug = 1024;

    private Texture2D textureDebug;
    private Sprite spriteDebug;
    private Coroutine detectionCoroutine;
    private TileType _lastTerrainValue = TileType.None;
    private MapData _mapData;

    public void Start()
    {
        if (mapRenderTexture != null) textureDebug = new Texture2D(mapRenderTexture.width, mapRenderTexture.height, TextureFormat.RGBA32, false);
        OnStartSession();
    }

    public void OnEnable()
    {
        ActionsManager.OnStartSession += OnStartSession;
        ActionsManager.OnPlayerRun += OnPlayerRun;
        ActionsManager.OnPlayerIdle += OnPlayerIdle;
    }
    
    public void OnDisable()
    {
        StopDetection();
        ActionsManager.OnStartSession -= OnStartSession;
        ActionsManager.OnPlayerRun -= OnPlayerRun;
        ActionsManager.OnPlayerIdle -= OnPlayerIdle;
    }

    public void OnStartSession()
    {
        _mapData = MapsManager.Instance.GetActualMap();
    }

    public void StopDetection()
    {
        if (detectionCoroutine != null)
        {
            StopCoroutine(detectionCoroutine);
            detectionCoroutine = null;
        }
    }

    public void OnPlayerRun()
    {
        StopDetection();
        detectionCoroutine = StartCoroutine(TryCopie());
    }

    public void OnPlayerIdle()
    {
        StopDetection();
    }

    public IEnumerator TryCopie()
    {
        while (true)
        {
            if (playerTransform == null || mapRendererTransform == null || mapRenderTexture == null)
                yield return new WaitForSeconds(detectionDelay);

            Color pixelColor = GetPixelColor();
            currentNoiseValue = pixelColor.r;
            testColor = pixelColor;

            ActionSelonTerrain(currentNoiseValue);
            yield return new WaitForSeconds(detectionDelay);
        }
    }

    public Color GetPixelColor()
    {
        RenderTexture current = RenderTexture.active;
        RenderTexture.active = mapRenderTexture;

        textureDebug.ReadPixels(new Rect(0, 0, mapRenderTexture.width, mapRenderTexture.height), 0, 0);
        textureDebug.Apply();

        if (testRenderer != null)
        {
            float regionWidth = textureDebug.width / zoomFactor;
            float regionHeight = textureDebug.height / zoomFactor;
            float offsetX = (textureDebug.width / 2f) - ((regionWidth / testX) / 2f);
            float offsetY = (textureDebug.height / 2f) - ((regionHeight / testY) / 2f);
            float safeWidth = Mathf.Min(regionWidth, textureDebug.width - offsetX);
            float safeHeight = Mathf.Min(regionHeight, textureDebug.height - offsetY);

            float correctedPPU = safeWidth / mapRendererTransform.lossyScale.x;

            spriteDebug = Sprite.Create(
                textureDebug,
                new Rect(offsetX, offsetY, safeWidth, safeHeight),
                new Vector2(0.5f, 0.5f),
                correctedPPU
            );
            testRenderer.sprite = spriteDebug;
        }

        // ----------------------------------------------------
        // Clamp the coordinates to ensure they are within the texture bounds
        xDebug = Mathf.Clamp(xDebug, 0, textureDebug.width - 1);
        yDebug = Mathf.Clamp(yDebug, 0, textureDebug.height - 1);

        Color color = textureDebug.GetPixel(xDebug, yDebug);

        RenderTexture.active = current;

        return color;
    }

    void ActionSelonTerrain(float noiseValue)
    {
        List<LayerRule> layerRules = _mapData.LayerRules;

        for(int i = 0; i < layerRules.Count; i++)
        {
            LayerRule rule = layerRules[i];
            if (noiseValue >= rule.threshold)
            {
                _terrainValue = rule.tileType;
                break;
            }
        }

        if (_lastTerrainValue != _terrainValue)
        {
            _lastTerrainValue = _terrainValue;
            ActionsManager.OnTerrainChange?.Invoke(_terrainValue);
        }
    }

    private void OnDestroy()
    {
        if (textureDebug != null) Destroy(textureDebug);
        if (spriteDebug != null) Destroy(spriteDebug);
    }
}