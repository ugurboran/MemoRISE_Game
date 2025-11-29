using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yonetimi icin

// Bu script, LevelData ScriptableObject'lerini kullanarak seviyeleri yonetir
// Seviyeleri yukler, platformlari olusturur ve oyun akisini kontrol eder
public class LevelManager : MonoBehaviour
{

    [Header("Seviye Sistemi")]
    [Tooltip("Oyundaki tum seviyeler sirali olarak")]
    public LevelData[] allLevels; // Tum seviye verileri

    [Tooltip("Yuklenecek seviye verisi (otomatik atanir)")]
    public LevelData currentLevel;

    private int currentLevelIndex = 0; // Su anki seviye indexi

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
        // Seviye dizisi kontrol
        if (allLevels == null || allLevels.Length == 0)
        {
            Debug.LogError("LevelManager: allLevels dizisi bos! Inspector'dan seviyeleri ata.");
            return;
        }

        if (platformPrefab == null)
        {
            Debug.LogError("LevelManager: platformPrefab atanmamis!");
            return;
        }

        // Ilk seviyeyi yukle
        currentLevelIndex = 0;
        currentLevel = allLevels[currentLevelIndex];
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

        // Finish noktasini konumlandir
        PositionFinishPoint();

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
    /// Finish noktasini LevelData'daki pozisyona konumlandirir
    /// </summary>
    private void PositionFinishPoint()
    {
        // Sahnede Finish tag'li obje bul
        GameObject finishPoint = GameObject.FindGameObjectWithTag("Finish");

        if (finishPoint != null)
        {
            finishPoint.transform.position = currentLevel.finishPosition;

            if (showDebugLogs)
            {
                Debug.Log($"  → Finish noktasi konumlandirildi: {currentLevel.finishPosition}");
            }
        }
        else
        {
            Debug.LogWarning("LevelManager: Finish tag'li obje bulunamadi!");
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
    /// <summary>
    /// Seviye tamamlandiginda cagrilir (FinishPoint tarafindan)
    /// </summary>
    public void OnLevelComplete()
    {
        if (showDebugLogs)
        {
            Debug.Log($"🎉 {currentLevel.levelName} tamamlandi!");
        }

        // Bir sonraki seviyeye gec
        LoadNextLevel();
    }

    /// <summary>
    /// Bir sonraki seviyeyi yukler
    /// </summary>
    private void LoadNextLevel()
    {
        currentLevelIndex++; // Index'i artir

        // Tum seviyeler tamamlandiysa
        if (currentLevelIndex >= allLevels.Length)
        {
            if (showDebugLogs)
            {
                Debug.Log("🏆 TUM SEVIYELER TAMAMLANDI!");
            }

            // Oyunu yeniden baslat veya baska bir islem yap
            RestartGame();
            return;
        }

        // Sonraki seviyeyi yukle
        currentLevel = allLevels[currentLevelIndex];

        if (showDebugLogs)
        {
            Debug.Log($"→ Sonraki seviye yukleniyor: {currentLevel.levelName}");
        }

        // Eski seviyeyi temizle ve yenisini yukle
        ClearCurrentLevel();
        LoadLevel();
    }

    /// <summary>
    /// Oyunu yeniden baslatir (ilk seviyeden)
    /// </summary>
    private void RestartGame()
    {
        if (showDebugLogs)
        {
            Debug.Log("🔄 Oyun yeniden baslatiliyor...");
        }

        // Mevcut sahneyi yeniden yukle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Belirli bir seviyeyi yukler (opsiyonel - UI'dan kullanilabilir)
    /// </summary>
    public void LoadSpecificLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= allLevels.Length)
        {
            Debug.LogError($"LevelManager: Gecersiz level index: {levelIndex}");
            return;
        }

        currentLevelIndex = levelIndex;
        currentLevel = allLevels[currentLevelIndex];

        ClearCurrentLevel();
        LoadLevel();
    }
}