using UnityEngine;

// Bu script, LevelData ScriptableObject'lerini kullanarak seviyeleri yonetir
// Seviyeleri yukler, platformlari olusturur ve oyun akisini kontrol eder
public class LevelManager : MonoBehaviour
{
    [Header("Seviye Sistemi")]
    [Tooltip("Yuklenecek seviye verisi")]
    public LevelData currentLevel;

    [Header("Prefab Referanslari")]
    [Tooltip("Platform prefab referansi")]
    public GameObject platformPrefab;

    [Tooltip("Oyuncu prefab referansi (opsiyonel)")]
    public GameObject playerPrefab;

    [Header("Debug")]
    [Tooltip("Platform olusturma surecini console'da goster")]
    public bool showDebugLogs = true;

    private GameObject playerInstance; // Sahnedeki oyuncu referansi

    private void Start()
    {
        // Seviye verisi kontrol
        if (currentLevel == null)
        {
            Debug.LogError("LevelManager: currentLevel atanmamis! Inspector'dan bir LevelData ata.");
            return;
        }

        if (platformPrefab == null)
        {
            Debug.LogError("LevelManager: platformPrefab atanmamis!");
            return;
        }

        // Seviyeyi yukle
        LoadLevel();
    }

    /// <summary>
    /// Mevcut seviyeyi yukler: platformlari olusturur, oyuncuyu konumlandirir
    /// </summary>
    private void LoadLevel()
    {
        if (showDebugLogs)
        {
            Debug.Log($"=== {currentLevel.levelName} yukleniyor ===");
        }

        // Platformlari olustur
        GeneratePlatforms();

        // Oyuncuyu baslangic pozisyonuna tasi
        PositionPlayer();

        if (showDebugLogs)
        {
            Debug.Log($"✅ {currentLevel.levelName} basariyla yuklendi!");
        }
    }

    /// <summary>
    /// LevelData'daki ayarlara gore platformlari olusturur
    /// </summary>
    private void GeneratePlatforms()
    {
        for (int i = 0; i < currentLevel.platformCount; i++)
        {
            // Random pozisyon hesapla (LevelData sinirlari icinde)
            float randomX = Random.Range(currentLevel.minX, currentLevel.maxX);
            float randomY = Random.Range(currentLevel.minY, currentLevel.maxY);
            Vector2 spawnPosition = new Vector2(randomX, randomY);

            // Platformu olustur
            GameObject platform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
            platform.name = $"Platform_{i + 1}";

            // PlatformReveal script'ini bul ve ayarlari uygula
            PlatformReveal reveal = platform.GetComponent<PlatformReveal>();
            if (reveal != null)
            {
                reveal.revealDuration = currentLevel.revealDuration;
            }

            // Platformu LevelManager altina organize et (Hierarchy'de duzgun gozuksun)
            platform.transform.SetParent(this.transform);
        }

        if (showDebugLogs)
        {
            Debug.Log($"  → {currentLevel.platformCount} platform olusturuldu (Gorunme suresi: {currentLevel.revealDuration}s)");
        }
    }

    /// <summary>
    /// Oyuncuyu LevelData'daki baslangic pozisyonuna tasir
    /// </summary>
    private void PositionPlayer()
    {
        // Sahnede var olan Player'i bul
        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");

        if (existingPlayer != null)
        {
            existingPlayer.transform.position = currentLevel.playerStartPosition;
            playerInstance = existingPlayer;

            if (showDebugLogs)
            {
                Debug.Log($"  → Oyuncu baslangic pozisyonuna tasindi: {currentLevel.playerStartPosition}");
            }
        }
        else if (playerPrefab != null)
        {
            // Sahnede Player yoksa ve prefab atanmissa, olustur
            playerInstance = Instantiate(playerPrefab, currentLevel.playerStartPosition, Quaternion.identity);
            playerInstance.name = "Player";

            if (showDebugLogs)
            {
                Debug.Log($"  → Oyuncu olusturuldu: {currentLevel.playerStartPosition}");
            }
        }
        else
        {
            Debug.LogWarning("LevelManager: Sahnede Player bulunamadi ve playerPrefab de atanmamis.");
        }
    }

    /// <summary>
    /// Yeni bir seviye yuklemek icin (sonraki seviyeye gecis)
    /// </summary>
    public void LoadNewLevel(LevelData newLevel)
    {
        // Onceki seviyeyi temizle
        ClearCurrentLevel();

        // Yeni seviyeyi yukle
        currentLevel = newLevel;
        LoadLevel();
    }

    /// <summary>
    /// Mevcut seviyeleri temizler (platformlari yok eder)
    /// </summary>
    private void ClearCurrentLevel()
    {
        // LevelManager altindaki tum platformlari yok et
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        if (showDebugLogs)
        {
            Debug.Log("  → Onceki seviye temizlendi.");
        }
    }
}