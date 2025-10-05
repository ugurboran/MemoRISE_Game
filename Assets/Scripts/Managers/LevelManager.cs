using UnityEngine;

// Automatically creates platforms in random positions (X and Y axes)
// and applies reveal duration settings to each.

public class LevelManager : MonoBehaviour
{
    [Header("Platform Settings")]
    [Tooltip("Platform prefab to be created")]
    public GameObject platformPrefab;

    [Tooltip("Number of platforms to create")]
    public int platformCount = 5;

    [Header("Random Position Settings")]
    [Tooltip("Maximum random offset on X axis from the start position")]
    public float randomRangeX = 5f;

    [Tooltip("Maximum random offset on Y axis from the start position")]
    public float randomRangeY = 2f;

    [Header("Reveal Settings")]
    [Tooltip("How many seconds platforms stay visible when scene starts")]
    public float revealDuration = 5f;

    [Tooltip("Center position where random placement starts around")]
    public Vector2 startPosition = new Vector2(0, 0);

    private void Start()
    {
        GeneratePlatforms();
    }

    private void GeneratePlatforms()
    {
        for (int i = 0; i < platformCount; i++)
        {
            // Random X and Y offsets
            float randomX = Random.Range(-randomRangeX, randomRangeX);
            float randomY = Random.Range(-randomRangeY, randomRangeY);

            // New random spawn position
            Vector2 spawnPosition = startPosition + new Vector2(randomX, randomY);

            // Instantiate platform
            GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
            platform.name = $"Platform_{i + 1}";

            // Apply reveal duration if PlatformReveal exists
            PlatformReveal reveal = platform.GetComponent<PlatformReveal>();
            if (reveal != null)
                reveal.revealDuration = revealDuration;
        }

        Debug.Log($"Created {platformCount} platforms at random positions (X±{randomRangeX}, Y±{randomRangeY}).");
    }
}
