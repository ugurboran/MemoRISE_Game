using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Automatically generates platforms in random positions (+X, +Y)
/// and applies reveal duration settings for each platform.
/// Minimum distance between platforms is maintained.
/// </summary>
public class LevelManager : MonoBehaviour
{
    [Header("Platform Settings")]
    [Tooltip("Platform prefab to be created")]
    public GameObject platformPrefab; // Platform prefab reference

    [Tooltip("Number of platforms to create")]
    public int platformCount = 5; // Total platforms to spawn

    [Header("Random Position Settings")]
    [Tooltip("Maximum random offset on X axis from start position (+X only)")]
    public float randomRangeX = 5f; // Max X offset for random placement

    [Tooltip("Maximum random offset on Y axis from start position (+Y only)")]
    public float randomRangeY = 2f; // Max Y offset for random placement

    [Tooltip("Minimum distance between platforms")]
    public float minDistance = 1.5f; // Inspector adjustable minimum distance

    [Header("Reveal Settings")]
    [Tooltip("How many seconds platforms stay visible when scene starts")]
    public float revealDuration = 5f; // Platform visible duration

    [Tooltip("Center position where random placement starts")]
    public Vector2 startPosition = new Vector2(0, 0); // Starting point for placement

    /// <summary>
    /// Unity Start method: called once when scene starts
    /// </summary>
    private void Start()
    {
        GeneratePlatforms(); // Generate all platforms at start
    }

    /// <summary>
    /// Generates platforms at random positions (+X, +Y),
    /// ensures minimum spacing between platforms
    /// </summary>
    private void GeneratePlatforms()
    {
        List<Vector2> placedPositions = new List<Vector2>(); // Store spawned positions

        for (int i = 0; i < platformCount; i++)
        {
            Vector2 spawnPosition;
            int attempts = 0;
            const int maxAttempts = 100; // Prevent infinite loops

            // Generate a valid random position
            do
            {
                float randomX = Random.Range(0f, randomRangeX); // Random X offset
                float randomY = Random.Range(0f, randomRangeY); // Random Y offset
                spawnPosition = startPosition + new Vector2(randomX, randomY);
                attempts++;
            }
            while (!IsPositionValid(spawnPosition, placedPositions) && attempts < maxAttempts);

            if (attempts >= maxAttempts)
            {
                Debug.LogWarning($"Platform {i + 1}: suitable position not found, using last generated position.");
            }

            // Instantiate the platform at calculated position
            GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
            platform.name = $"Platform_{i + 1}";

            // Apply revealDuration from LevelManager to PlatformReveal script
            PlatformReveal reveal = platform.GetComponent<PlatformReveal>();
            if (reveal != null)
                reveal.revealDuration = revealDuration;

            // Add position to list for future distance checks
            placedPositions.Add(spawnPosition);
        }

        Debug.Log($"Created {platformCount} platforms at random positions (+X, +Y) with minDistance {minDistance}.");
    }

    /// <summary>
    /// Checks if the new position is far enough from existing platforms
    /// </summary>
    /// <param name="newPos">Candidate position</param>
    /// <param name="existingPositions">Already placed positions</param>
    /// <returns>True if valid, False if too close</returns>
    private bool IsPositionValid(Vector2 newPos, List<Vector2> existingPositions)
    {
        foreach (Vector2 pos in existingPositions)
        {
            if (Vector2.Distance(newPos, pos) < minDistance)
                return false; // Too close to existing platform
        }
        return true; // Valid position
    }
}
