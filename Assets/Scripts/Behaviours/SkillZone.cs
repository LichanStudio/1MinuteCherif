using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(LineRenderer))]
public class SkillZone : MonoBehaviour
{
    [Header("Configuration")]
    public Color FillColor = Color.red;

    [Header("Bordure")]
    public Color borderColor = Color.white;
    public float borderWidth = 0.1f;
    public Material borderMaterial;

    [Range(0f, 1f)]
    public float FillProgress = 0f;
    public float FillSpeed = 1f;
    public float TimeProcAfterFill = 0.5f;

    private Material fillMaterial;
    private float minY;
    private float maxY;

    private PolygonCollider2D _polyCollider2D;
    private MeshFilter _filter;
    private MeshRenderer _renderer;

    void Start()
    {
        _polyCollider2D = GetComponent<PolygonCollider2D>();
        _filter = GetComponent<MeshFilter>();
        _renderer = GetComponent<MeshRenderer>();
        GenerateMeshAndSetupMaterial();
        SetupBorder(_polyCollider2D);
    }

    public void OnEnable()
    {
        AnimateFill(1.0f);
    }

    void Update()
    {
        if (fillMaterial != null) fillMaterial.SetFloat("_Progress", FillProgress);
    }

    public void GenerateMeshAndSetupMaterial()
    {
        Vector2[] points = _polyCollider2D.points;

        if (points.Length < 3) return;

        // 1. Calculer la hauteur minimale et maximale (Base vs Haut)
        minY = float.MaxValue;
        maxY = float.MinValue;

        Vector3[] vertices = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            vertices[i] = new Vector3(points[i].x, points[i].y, 0);
            if (points[i].y < minY) minY = points[i].y;
            if (points[i].y > maxY) maxY = points[i].y;
        }

        // 2. Générer le Mesh avec Triangulator
        Triangulator tr = new(points);
        int[] indices = tr.Triangulate();

        Mesh mesh = new()
        {
            vertices = vertices,
            triangles = indices
        };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        _filter.mesh = mesh;

        // 3. Appliquer le Shader personnalisé
        Shader customShader = Shader.Find("Custom/ProgressFill2D");
        if (customShader == null) return;

        fillMaterial = new Material(customShader);
        fillMaterial.SetColor("_Color", FillColor);
        fillMaterial.SetFloat("_MinY", minY);
        fillMaterial.SetFloat("_MaxY", maxY);
        fillMaterial.SetFloat("_Progress", FillProgress);

        _renderer.material = fillMaterial;
    }

    void SetupBorder(PolygonCollider2D poly)
    {
        LineRenderer lr = GetComponent<LineRenderer>();

        // Configuration de base
        lr.useWorldSpace = false; // Important : dessiner en local
        lr.loop = true; // Pour fermer le contour

        // Matériau et couleurs
        if (borderMaterial != null) lr.material = borderMaterial;
        else lr.material = new Material(Shader.Find("Sprites/Default"));

        lr.startColor = borderColor;
        lr.endColor = borderColor;

        // Largeur
        lr.startWidth = borderWidth;
        lr.endWidth = borderWidth;

        // Copier les points du collider
        Vector2[] points = poly.points;
        lr.positionCount = points.Length;
        for (int i = 0; i < points.Length; i++)
        {
            // Convertir en Vector3 pour le LineRenderer (Z=0)
            lr.SetPosition(i, new Vector3(points[i].x, points[i].y, 0));
        }
    }

    public void AnimateFill(float targetProgress)
    {
        StartCoroutine(FillRoutine(targetProgress));
    }

    private System.Collections.IEnumerator FillRoutine(float target)
    {
        FillProgress = 0f;
        while (!Mathf.Approximately(FillProgress, target))
        {
            FillProgress = Mathf.MoveTowards(FillProgress, target, Time.deltaTime * FillSpeed);
            yield return null;
        }
        yield return new WaitForSeconds(TimeProcAfterFill);
        List<Collider2D> entitiesInZone = GetEntitiesInZone();
        for (int i = 0; i < entitiesInZone.Count; i++)
        {
            Debug.Log(entitiesInZone[i].name);
            if (entitiesInZone[i] != null && entitiesInZone[i].TryGetComponent(out EntityScript entityScript)) {
                ActionsManager.OnDamageEntity?.Invoke(entityScript, 10);
            }
        }

        Destroy(gameObject);
    }

    public List<Collider2D> GetEntitiesInZone()
    {
        ContactFilter2D filter = new();
        List<Collider2D> results = new();
        _polyCollider2D.Overlap(filter, results);
        return results;
    }
}