using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public static class ContourColliderBuilder
{
    /// <summary>
    /// Construit un PolygonCollider2D avec un path par îlot d'eau détecté.
    /// </summary>
    public static void BuildZonesContours(
        NativeArray<int> tileMap, int chunkSize,
        float pixelSize,
        Dictionary<int, PolygonCollider2D> collidersByRuleIndex)
    {
        // Récupère tous les types présents dans la map
        var uniqueRules = new HashSet<int>();
        for (int i = 0; i < tileMap.Length; i++)
            uniqueRules.Add(tileMap[i]);

        foreach (int ruleIndex in uniqueRules)
        {
            if (!collidersByRuleIndex.TryGetValue(ruleIndex, out var poly)) continue;

            // Grille booléenne pour ce type
            bool[] grid = new bool[chunkSize * chunkSize];
            for (int i = 0; i < tileMap.Length; i++)
                grid[i] = tileMap[i] == ruleIndex;

            // Flood fill + contours
            bool[] visited = new bool[chunkSize * chunkSize];
            var paths = new List<Vector2[]>();

            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    int idx = y * chunkSize + x;
                    if (!grid[idx] || visited[idx]) continue;

                    List<Vector2Int> island = FloodFill(grid, visited, x, y, chunkSize);
                    Vector2[] contour = MarchingSquaresContour(island, chunkSize, pixelSize);
                    if (contour != null && contour.Length >= 3)
                        paths.Add(contour);
                }
            }

            poly.pathCount = paths.Count;
            if (paths.Count > 0) poly.gameObject.SetActive(true);
            for (int i = 0; i < paths.Count; i++)
                poly.SetPath(i, paths[i]);
        }
    }

    // ------------------------------------------------------------------
    // Flood Fill : collecte tous les pixels contigus d'un même îlot
    // ------------------------------------------------------------------
    private static List<Vector2Int> FloodFill(
        bool[] grid, bool[] visited, int startX, int startY, int chunkSize)
    {
        var result = new List<Vector2Int>();
        var stack = new Stack<Vector2Int>();
        stack.Push(new Vector2Int(startX, startY));

        while (stack.Count > 0)
        {
            var p = stack.Pop();
            int idx = p.y * chunkSize + p.x;

            if (p.x < 0 || p.x >= chunkSize || p.y < 0 || p.y >= chunkSize) continue;
            if (!grid[idx] || visited[idx]) continue;

            visited[idx] = true;
            result.Add(p);

            stack.Push(new Vector2Int(p.x + 1, p.y));
            stack.Push(new Vector2Int(p.x - 1, p.y));
            stack.Push(new Vector2Int(p.x, p.y + 1));
            stack.Push(new Vector2Int(p.x, p.y - 1));
        }
        return result;
    }

    // ------------------------------------------------------------------
    // Marching Squares : extrait le contour vectoriel d'un îlot
    // ------------------------------------------------------------------
    private static Vector2[] MarchingSquaresContour(
        List<Vector2Int> island, int chunkSize, float pixelSize)
    {
        // Grille locale de l'îlot
        var set = new HashSet<Vector2Int>(island);

        // Point de départ : pixel le plus bas-gauche (ordre de parcours du tileMap)
        Vector2Int start = island[0];

        // Direction : on tourne dans le sens horaire autour du contour
        // On utilise l'algorithme "Moore neighborhood" (contour tracing)
        var contour = new List<Vector2>();
        var edgeSet = new HashSet<(Vector2Int, int)>(); // (pixel, edge) déjà traités

        // Les 4 arêtes d'un pixel : 0=bas, 1=droite, 2=haut, 3=gauche
        Vector2Int[] neighbors = {
            new( 0, -1), // bas
            new( 1,  0), // droite
            new( 0,  1), // haut
            new(-1,  0), // gauche
        };

        // Coins de chaque arête (en coordonnées de pixel, sens horaire)
        //  pixel origin = bas-gauche
        Vector2[,] edgeVerts = {
            { new(0,0), new(1,0) }, // bas   : BG -> BD
            { new(1,0), new(1,1) }, // droite: BD -> HD
            { new(1,1), new(0,1) }, // haut  : HD -> HG
            { new(0,1), new(0,0) }, // gauche: HG -> BG
        };

        // Collecter toutes les arêtes de bordure (arête entre un pixel eau et non-eau)
        var borderEdges = new List<(Vector2Int pixel, int edge)>();
        foreach (var p in island)
        {
            for (int e = 0; e < 4; e++)
            {
                Vector2Int neighbor = p + neighbors[e];
                if (!set.Contains(neighbor))
                    borderEdges.Add((p, e));
            }
        }

        if (borderEdges.Count == 0) return null;

        // Construire un graphe d'adjacence des segments de contour
        // pour les chaîner en polygone(s)
        // Chaque segment = (v0, v1) en coordonnées monde
        var segments = new List<(Vector2 a, Vector2 b)>();
        foreach (var (pixel, edge) in borderEdges)
        {
            Vector2 v0 = new(
                (pixel.x + edgeVerts[edge, 0].x) * pixelSize,
                (pixel.y + edgeVerts[edge, 0].y) * pixelSize);
            Vector2 v1 = new(
                (pixel.x + edgeVerts[edge, 1].x) * pixelSize,
                (pixel.y + edgeVerts[edge, 1].y) * pixelSize);
            segments.Add((v0, v1));
        }

        return ChainSegments(segments);
    }

    // ------------------------------------------------------------------
    // Chaîne les segments de bordure en un polygone ordonné
    // ------------------------------------------------------------------
    private static Vector2[] ChainSegments(List<(Vector2 a, Vector2 b)> segments)
    {
        if (segments.Count == 0) return null;

        var result = new List<Vector2>();
        var remaining = new List<(Vector2 a, Vector2 b)>(segments);

        result.Add(remaining[0].a);
        Vector2 current = remaining[0].b;
        remaining.RemoveAt(0);

        const float eps = 0.0001f;

        while (remaining.Count > 0)
        {
            bool found = false;
            for (int i = 0; i < remaining.Count; i++)
            {
                var (a, b) = remaining[i];
                if (Vector2.Distance(current, a) < eps)
                {
                    result.Add(a);
                    current = b;
                    remaining.RemoveAt(i);
                    found = true;
                    break;
                }
                if (Vector2.Distance(current, b) < eps)
                {
                    result.Add(b);
                    current = a;
                    remaining.RemoveAt(i);
                    found = true;
                    break;
                }
            }
            if (!found) break; // îlot non connexe, on arrête
        }

        return result.ToArray();
    }
}