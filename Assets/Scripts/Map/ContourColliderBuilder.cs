using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public static class ContourColliderBuilder
{
    // Tableaux de travail réutilisés pour éviter le Garbage Collector
    private static bool[] _gridCache;
    private static bool[] _visitedCache;
    private static readonly Stack<Vector2Int> _floodFillStack = new();
    private static readonly List<Vector2Int> _currentIsland = new();
    private static readonly List<Vector2> _orderedPoints = new();

    private static void AllocateCaches(int size)
    {
        int total = size * size;
        if (_gridCache == null || _gridCache.Length < total)
        {
            _gridCache = new bool[total];
            _visitedCache = new bool[total];
        }
    }

    public static void BuildZonesContours(
        NativeArray<int> tileMap, int chunkSize, float pixelSize,
        Dictionary<int, PolygonCollider2D> collidersByRuleIndex)
    {
        AllocateCaches(chunkSize);

        // Étape 1 : Trouver les règles uniques sans réallouer de HashSet
        // On se base sur le dictionnaire existant passé en paramètre
        foreach (var pair in collidersByRuleIndex)
        {
            int ruleIndex = pair.Key;
            PolygonCollider2D poly = pair.Value;

            // Préparer la grille booléenne pour cette règle
            int totalPixels = chunkSize * chunkSize;
            bool ruleExists = false;

            for (int i = 0; i < totalPixels; i++)
            {
                bool isMatch = tileMap[i] == ruleIndex;
                _gridCache[i] = isMatch;
                _visitedCache[i] = false;
                if (isMatch) ruleExists = true;
            }

            if (!ruleExists) continue;

            var paths = new List<Vector2[]>();

            // Étape 2 : Flood fill + Moore-Neighborhood Trace (Ordre Linéaire)
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    int idx = y * chunkSize + x;
                    if (!_gridCache[idx] || _visitedCache[idx]) continue;

                    // Extrait l'île (remplit _currentIsland)
                    FloodFillOptimized(chunkSize, x, y);

                    // Génère le contour DIRECTEMENT ordonné en O(N)
                    Vector2[] contour = TraceContourLinear(chunkSize, pixelSize);

                    if (contour != null && contour.Length >= 3)
                    {
                        paths.Add(contour);
                    }
                }
            }

            // Étape 3 : Application physique
            poly.pathCount = paths.Count;
            if (paths.Count > 0) poly.gameObject.SetActive(true);
            for (int i = 0; i < paths.Count; i++)
            {
                poly.SetPath(i, paths[i]);
            }
        }
    }

    private static void FloodFillOptimized(int chunkSize, int startX, int startY)
    {
        _currentIsland.Clear();
        _floodFillStack.Clear();
        _floodFillStack.Push(new Vector2Int(startX, startY));

        while (_floodFillStack.Count > 0)
        {
            var p = _floodFillStack.Pop();
            if (p.x < 0 || p.x >= chunkSize || p.y < 0 || p.y >= chunkSize) continue;

            int idx = p.y * chunkSize + p.x;
            if (!_gridCache[idx] || _visitedCache[idx]) continue;

            _visitedCache[idx] = true;
            _currentIsland.Add(p);

            _floodFillStack.Push(new Vector2Int(p.x + 1, p.y));
            _floodFillStack.Push(new Vector2Int(p.x - 1, p.y));
            _floodFillStack.Push(new Vector2Int(p.x, p.y + 1));
            _floodFillStack.Push(new Vector2Int(p.x, p.y - 1));
        }
    }

    // Suivi de contour de Moore (Moore Neighborhood Tracer) : Complexité O(N) au lieu de O(N²)
    private static Vector2[] TraceContourLinear(int chunkSize, float pixelSize)
    {
        if (_currentIsland.Count == 0) return null;

        _orderedPoints.Clear();

        Vector2Int startPixel = _currentIsland[0];
        Vector2Int currentPixel = startPixel;

        // Directions : 0=Nord, 1=Est, 2=Sud, 3=Ouest
        Vector2Int[] dirOffsets = { new(0, 1), new(1, 0), new(0, -1), new(-1, 0) };
        int backtrackDir = 3;

        // ⭐ NOUVEAU : Variables pour suivre la direction à la volée
        Vector2Int lastDirection = Vector2Int.zero;
        bool firstPointAdded = false;

        int maxIterations = _currentIsland.Count * 4;
        int iterations = 0;

        do
        {
            Vector2Int nextPixel = Vector2Int.zero;
            bool foundNext = false;

            for (int i = 0; i < 4; i++)
            {
                int evalDir = (backtrackDir + i) % 4;
                Vector2Int candidate = currentPixel + dirOffsets[evalDir];

                if (candidate.x >= 0 && candidate.x < chunkSize && candidate.y >= 0 && candidate.y < chunkSize)
                {
                    int idx = candidate.y * chunkSize + candidate.x;
                    if (_gridCache[idx] && _visitedCache[idx])
                    {
                        nextPixel = candidate;
                        backtrackDir = (evalDir + 3) % 4;
                        foundNext = true;
                        break;
                    }
                }
            }

            if (!foundNext) break;

            // ⭐ NOUVEAU : Calcul de la direction du pas actuel
            Vector2Int currentDirection = nextPixel - currentPixel;

            // Si la direction change, le pixel sur lequel on est (currentPixel) est un COIN !
            if (firstPointAdded && currentDirection != lastDirection)
            {
                _orderedPoints.Add(new Vector2(currentPixel.x * pixelSize, currentPixel.y * pixelSize));
            }
            else if (!firstPointAdded)
            {
                // On force l'enregistrement du tout premier point de l'île
                _orderedPoints.Add(new Vector2(currentPixel.x * pixelSize, currentPixel.y * pixelSize));
                firstPointAdded = true;
            }

            lastDirection = currentDirection;
            currentPixel = nextPixel;
            iterations++;

        } while (currentPixel != startPixel && iterations < maxIterations);

        // Toujours fermer proprement le polygone en rajoutant le point de départ à la fin
        _orderedPoints.Add(new Vector2(startPixel.x * pixelSize, startPixel.y * pixelSize));

        // ⭐ NOUVEAU : Application d'un lissage avancé basé sur l'epsilon
        // eps = bande d'erreur autorisée. Augmentez la si le lissage est trop faible (ex: 0.1), diminuez la s'il est trop fort.
        float eps = 0.01f;
        return SimplifyPolygonWithEpsilon(_orderedPoints, eps);
    }

    /// <summary>
    /// Simplifie un polygone brut de Moore pour lisser les bords pixélisés en escalier.
    /// Élimine les points qui ne changent pas la géométrie de manière significative.
    /// </summary>
    private static Vector2[] SimplifyPolygonWithEpsilon(List<Vector2> points, float eps)
    {
        if (points.Count < 3) return points.ToArray();

        List<Vector2> simplified = new List<Vector2>();

        // Toujours garder le tout premier point
        simplified.Add(points[0]);

        // Points intermédiaires de Moore
        for (int i = 1; i < points.Count - 1; i++)
        {
            Vector2 pPrevious = simplified[simplified.Count - 1];
            Vector2 pCurrent = points[i];
            Vector2 pNext = points[i + 1];

            // 1. Calcul du cross-product classique pour détecter les alignements parfaits (colinéaires)
            // Cela simplifie les grandes lignes droites sur l'axe X ou Y.
            float area = (pCurrent.y - pPrevious.y) * (pNext.x - pCurrent.x) -
                         (pCurrent.x - pPrevious.x) * (pNext.y - pCurrent.y);

            bool perfectlyColinear = Mathf.Abs(area) < 0.0001f;

            // 2. Calcul du lissage basé sur l'epsilon (Douglas-Peucker simplifié/Heuristique d'élagage)
            // On calcule si le point 'pCurrent' est proche de la ligne qui relie 'pPrevious' à 'pNext'.
            bool nearLineApprox = false;
            if (!perfectlyColinear)
            {
                // Si ce n'est pas parfaitement aligné, on vérifie si c'est un "mini-pas d'escalier"
                // Heuristique simple : si le changement de direction est très faible, on fusionne.
                Vector2 dirPrev = (pCurrent - pPrevious).normalized;
                Vector2 dirNext = (pNext - pCurrent).normalized;
                nearLineApprox = Vector2.Dot(dirPrev, dirNext) > (1f - eps);
            }

            // Si c'est un vrai virage (parfaitement colinéaire ou nearLineApprox sont faux), on garde le point.
            if (!perfectlyColinear && !nearLineApprox)
            {
                simplified.Add(pCurrent);
            }
        }

        // Toujours garder le tout dernier point
        simplified.Add(points[points.Count - 1]);

        return simplified.ToArray();
    }
}