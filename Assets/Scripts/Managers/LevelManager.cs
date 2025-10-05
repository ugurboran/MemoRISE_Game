using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [Header("Platform Settings")]
    [Tooltip("Platform prefab to be created")]
    public GameObject platformPrefab;

    [Tooltip("Number of platforms to create")]
    public int platformCount = 5;

    [Header("Random Position Settings")]
    [Tooltip("Maximum random offset on X axis from start position (+X only)")]
    public float randomRangeX = 5f;

    [Tooltip("Maximum random offset on Y axis from start position (+Y only)")]
    public float randomRangeY = 2f;

    [Tooltip("Minimum distance between platforms")]
    public float minDistance = 1.5f; // Inspector’dan ayarlanabilir

    [Header("Reveal Settings")]
    [Tooltip("How many seconds platforms stay visible when scene starts")]
    public float revealDuration = 5f;

    [Tooltip("Center position where random placement starts")]
    public Vector2 startPosition = new Vector2(0, 0);

    private void Start()
    {
        GeneratePlatforms();
    }

    private void GeneratePlatforms()
    {
        List<Vector2> placedPositions = new List<Vector2>();

        for (int i = 0; i < platformCount; i++)
        {
            Vector2 spawnPosition;
            int attempts = 0;
            const int maxAttempts = 100;

            do
            {
                float randomX = Random.Range(0f, randomRangeX);
                float randomY = Random.Range(0f, randomRangeY);
                spawnPosition = startPosition + new Vector2(randomX, randomY);
                attempts++;
            }
            while (!IsPositionValid(spawnPosition, placedPositions) && attempts < maxAttempts);

            if (attempts >= maxAttempts)
            {
                Debug.LogWarning($"Platform {i + 1}: suitable position not found, using last generated position.");
            }

            GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
            platform.name = $"Platform_{i + 1}";

            PlatformReveal reveal = platform.GetComponent<PlatformReveal>();
            if (reveal != null)
                reveal.revealDuration = revealDuration;

            placedPositions.Add(spawnPosition);
        }

        Debug.Log($"Created {platformCount} platforms at random positions (+X, +Y) with minDistance {minDistance}.");
    }

    private bool IsPositionValid(Vector2 newPos, List<Vector2> existingPositions)
    {
        foreach (Vector2 pos in existingPositions)
        {
            if (Vector2.Distance(newPos, pos) < minDistance)
                return false;
        }
        return true;
    }
}
