using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yonetimi icin

// Bu script, LevelData ScriptableObject'lerini kullanarak seviyeleri yonetir
// Seviyeleri yukler, platformlari olusturur ve oyun akisini kontrol eder
public class LevelManager : MonoBehaviour
{
    private Vector2 firstPlatformPosition; // Ilk platformun pozisyonu (Player icin)
    

    [Header("Seviye Sistemi")]
    [Tooltip("Oyundaki tum seviyeler sirali olarak")]
    public LevelData[] allLevels; // Tum seviye verileri

    [Tooltip("Yuklenecek seviye verisi (otomatik atanir)")]
    public LevelData currentLevel;

    private int currentLevelIndex = 0; // Su anki seviye indexi

    private Vector2 lastPlatformPosition; // Son platformun pozisyonunu tutar

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
    /// LevelData'daki ayarlara gore platformlari zincir seklinde olusturur
    /// Ilk platform LevelData'daki pozisyondan baslar
    /// </summary>
    private void GeneratePlatforms()
    {
        // ILK PLATFORM: LevelData'daki pozisyonda olustur
        firstPlatformPosition = currentLevel.firstPlatformPosition;

        GameObject firstPlatform = Instantiate(platformPrefab, firstPlatformPosition, Quaternion.identity);
        firstPlatform.name = "Platform_1";

        PlatformReveal firstReveal = firstPlatform.GetComponent<PlatformReveal>();
        if (firstReveal != null)
        {
            firstReveal.revealDuration = currentLevel.revealDuration;
        }
        firstPlatform.transform.SetParent(this.transform);

        // Bir sonraki platform icin referans pozisyonu
        Vector2 currentPosition = firstPlatformPosition;

        // KALAN PLATFORMLAR: Zincir seklinde olustur
        for (int i = 1; i < currentLevel.platformCount; i++)
        {
            // Bir sonraki platformun pozisyonunu hesapla
            Vector2 nextPosition = CalculateNextPlatformPosition(currentPosition);

            // Platformu olustur
            GameObject platform = Instantiate(platformPrefab, nextPosition, Quaternion.identity);
            platform.name = $"Platform_{i + 1}";

            // PlatformReveal script'ini bul ve ayarlari uygula
            PlatformReveal reveal = platform.GetComponent<PlatformReveal>();
            if (reveal != null)
            {
                reveal.revealDuration = currentLevel.revealDuration;
            }

            // Platformu LevelManager altina organize et
            platform.transform.SetParent(this.transform);

            // Bir sonraki platform icin referans pozisyonu guncelle
            currentPosition = nextPosition;
        }

        // SON PLATFORMUN pozisyonunu kaydet (Finish Point icin)
        lastPlatformPosition = currentPosition;

        if (showDebugLogs)
        {
            Debug.Log($"  → {currentLevel.platformCount} platform zincir seklinde olusturuldu");
            Debug.Log($"  → Ilk platform: {firstPlatformPosition}, Son platform: {lastPlatformPosition}");
        }
    }

    /// <summary>
    /// Bir onceki platforma gore ziplanabilir mesafede yeni pozisyon hesaplar
    /// </summary>
    private Vector2 CalculateNextPlatformPosition(Vector2 previousPosition)
    {
        // Ziplanabilir mesafe araliginda random mesafe sec
        float jumpDistance = Random.Range(currentLevel.minJumpDistance, currentLevel.maxJumpDistance);

        // Yukseklik farki (Y ekseni varyasyonu)
        float heightDifference = Random.Range(-currentLevel.maxHeightVariation, currentLevel.maxHeightVariation);

        Vector2 nextPosition;

        if (currentLevel.horizontalPath)
        {
            // Yatay yol (saga dogru ilerleyen platformlar)
            nextPosition = new Vector2(
                previousPosition.x + jumpDistance, // Saga dogru mesafe ekle
                previousPosition.y + heightDifference // Y'de kucuk varyasyon
            );
        }
        else
        {
            // Dikey yol (yukari dogru ilerleyen platformlar)
            nextPosition = new Vector2(
                previousPosition.x + heightDifference, // X'de kucuk varyasyon
                previousPosition.y + jumpDistance // Yukari dogru mesafe ekle
            );
        }

        return nextPosition;
    }

    /// <summary>
    /// Finish noktasini son platformun uzerine/yakinina konumlandirir
    /// </summary>
    private void PositionFinishPoint()
    {
        // Sahnede Finish tag'li obje bul
        GameObject finishPoint = GameObject.FindGameObjectWithTag("Finish");

        if (finishPoint != null)
        {
            // Son platform pozisyonu + offset
            Vector2 finishPosition = lastPlatformPosition + currentLevel.finishOffsetFromLastPlatform;
            finishPoint.transform.position = finishPosition;

            // FinishPoint'i sifirla (yeni seviye icin hazir hale getir)
            FinishPoint finishScript = finishPoint.GetComponent<FinishPoint>();
            if (finishScript != null)
            {
                finishScript.ResetFinishPoint();
            }

            if (showDebugLogs)
            {
                Debug.Log($"  → Finish Point son platformun uzerine konumlandirildi: {finishPosition}");
            }
        }
        else
        {
            Debug.LogWarning("LevelManager: Finish tag'li obje bulunamadi!");
        }
    }

    /// <summary>
    /// Oyuncuyu ilk platformun uzerine yerlestirir
    /// </summary>
    private void PositionPlayer()
    {
        // Sahnede var olan Player'i bul
        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");

        // Ilk platformun 1 birim uzerinde spawn pozisyonu
        Vector2 playerSpawnPosition = firstPlatformPosition + new Vector2(0f, 1f);

        if (existingPlayer != null)
        {
            existingPlayer.transform.position = playerSpawnPosition;
            playerInstance = existingPlayer;

            if (showDebugLogs)
            {
                Debug.Log($"  → Oyuncu ilk platformun uzerine tasindi: {playerSpawnPosition}");
            }
        }
        else if (playerPrefab != null)
        {
            // Sahnede Player yoksa ve prefab atanmissa, olustur
            playerInstance = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
            playerInstance.name = "Player";

            if (showDebugLogs)
            {
                Debug.Log($"  → Oyuncu ilk platformun uzerine olusturuldu: {playerSpawnPosition}");
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
        Debug.Log($"[LevelManager] ClearCurrentLevel() baslatildi. LevelManager altinda {transform.childCount} obje var.");

        // LevelManager altindaki tum platformlari yok et
        int destroyedCount = 0;
        foreach (Transform child in transform)
        {
            Debug.Log($"[LevelManager] Yok ediliyor: {child.name}");
            Destroy(child.gameObject);
            destroyedCount++;
        }

        Debug.Log($"[LevelManager] {destroyedCount} platform yok edildi.");

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
        Debug.Log($"[LevelManager] OnLevelComplete() cagirildi!");
        Debug.Log($"[LevelManager] Mevcut seviye: {currentLevel.levelName} (Index: {currentLevelIndex})");

        if (showDebugLogs)
        {
            Debug.Log($"🎉 {currentLevel.levelName} tamamlandi!");
        }

        // Bir sonraki seviyeye gec
        Debug.Log("[LevelManager] LoadNextLevel() cagiriliyor...");
        LoadNextLevel();
    }

    /// <summary>
    /// Bir sonraki seviyeyi yukler
    /// </summary>
    private void LoadNextLevel()
    {
        Debug.Log($"[LevelManager] LoadNextLevel() baslatildi. Mevcut index: {currentLevelIndex}");

        currentLevelIndex++; // Index'i artir

        Debug.Log($"[LevelManager] Yeni index: {currentLevelIndex}, Toplam seviye sayisi: {allLevels.Length}");

        // Tum seviyeler tamamlandiysa
        if (currentLevelIndex >= allLevels.Length)
        {
            Debug.Log("[LevelManager] TUM SEVIYELER TAMAMLANDI!");

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

        Debug.Log($"[LevelManager] Sonraki seviye secildi: {currentLevel.levelName}");

        if (showDebugLogs)
        {
            Debug.Log($"→ Sonraki seviye yukleniyor: {currentLevel.levelName}");
        }

        // Eski seviyeyi temizle ve yenisini yukle
        Debug.Log("[LevelManager] ClearCurrentLevel() cagiriliyor...");
        ClearCurrentLevel();

        Debug.Log("[LevelManager] LoadLevel() cagiriliyor...");
        LoadLevel();

        Debug.Log("[LevelManager] Seviye gecisi tamamlandi!");
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



    /// <summary>
    /// Scene view'da platformlar arasi baglantilari ve ozel noktalari goster (Debug)
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Transform[] platforms = GetComponentsInChildren<Transform>();

        if (platforms.Length <= 1) return;

        // Platformlar arasi baglanti cizgileri (Sari)
        Gizmos.color = Color.yellow;
        for (int i = 1; i < platforms.Length - 1; i++)
        {
            Gizmos.DrawLine(platforms[i].position, platforms[i + 1].position);
        }

        // ILK PLATFORM (Yesil)
        if (platforms.Length > 1)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(platforms[1].position, 0.8f);
        }

        // SON PLATFORM (Kirmizi)
        if (platforms.Length > 2)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(platforms[platforms.Length - 1].position, 0.8f);
        }

        // PLAYER pozisyonu (Mavi nokta)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(player.transform.position, 0.6f);
        }

        // SON PLATFORM → FINISH baglantisi (Turuncu)
        GameObject finish = GameObject.FindGameObjectWithTag("Finish");
        if (finish != null && platforms.Length > 2)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f);
            Gizmos.DrawLine(platforms[platforms.Length - 1].position, finish.transform.position);
        }
    }
}