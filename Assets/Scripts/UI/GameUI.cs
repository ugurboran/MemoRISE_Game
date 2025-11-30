using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro icin

// Oyun ici UI'i yoneten merkezi script
// Seviye adi, sure, olum sayisi gibi bilgileri gosterir
public class GameUI : MonoBehaviour
{
    [Header("HUD Referanslari")]
    [Tooltip("Seviye adini gosteren text")]
    public TextMeshProUGUI levelNameText;

    [Tooltip("Sure sayacini gosteren text")]
    public TextMeshProUGUI timerText;

    [Tooltip("Olum sayisini gosteren text")]
    public TextMeshProUGUI deathCountText;

    [Header("Level Complete Panel")]
    [Tooltip("Seviye tamamlama ekrani")]
    public GameObject levelCompletePanel;

    [Tooltip("Seviye tamamlama mesaji")]
    public TextMeshProUGUI levelCompleteMessageText;

    [Tooltip("Istatistikler (sure, olum sayisi vs)")]
    public TextMeshProUGUI levelCompleteStatsText;

    [Header("Ayarlar")]
    [Tooltip("Sure sayaci aktif mi? (LevelData'dan otomatik alinir)")]
    public bool isTimerActive = false;

    private float currentTime = 0f; // Gecen sure
    private float timeLimit = 0f; // Sure limiti (eger varsa)
    private LevelManager levelManager; // LevelManager referansi

    private void Start()
    {
        // LevelManager'i bul
        levelManager = FindFirstObjectByType<LevelManager>();

        if (levelManager == null)
        {
            Debug.LogError("GameUI: LevelManager bulunamadi!");
            return;
        }

        // Level Complete Panel'i kapat
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }

        // UI'i ilk defa guncelle
        UpdateUI();
    }

    private void Update()
    {
        // Sure sayacini guncelle (aktifse)
        if (isTimerActive)
        {
            currentTime += Time.deltaTime;
            UpdateTimerDisplay();

            // Sure limiti varsa kontrol et
            if (timeLimit > 0 && currentTime >= timeLimit)
            {
                OnTimeUp();
            }
        }
    }

    /// <summary>
    /// UI'i yeni seviye bilgilerine gore gunceller
    /// </summary>
    public void UpdateUI()
    {
        if (levelManager == null) return;

        // Seviye adini guncelle
        if (levelNameText != null)
        {
            levelNameText.text = levelManager.currentLevel.levelName;
        }

        // Sure sayacini ayarla
        isTimerActive = levelManager.currentLevel.hasTimeLimit;
        if (isTimerActive)
        {
            timeLimit = levelManager.currentLevel.timeLimit;
            currentTime = 0f;
        }

        // Timer text'i goster/gizle
        if (timerText != null)
        {
            timerText.gameObject.SetActive(isTimerActive);
        }

        // Olum sayisini guncelle
        UpdateDeathCount();
    }

    /// <summary>
    /// Sure gosterimini gunceller
    /// </summary>
    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        if (timeLimit > 0)
        {
            // Geri sayim modu (kalan sure)
            float remainingTime = timeLimit - currentTime;
            if (remainingTime < 0) remainingTime = 0;

            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            timerText.text = $"⏱ {minutes:00}:{seconds:00}";

            // Son 10 saniyede kirmizi yap
            if (remainingTime <= 10f)
            {
                timerText.color = Color.red;
            }
            else
            {
                timerText.color = Color.white;
            }
        }
        else
        {
            // Normal sure gosterimi (gecen sure)
            int minutes = Mathf.FloorToInt(currentTime / 60f);
            int seconds = Mathf.FloorToInt(currentTime % 60f);
            timerText.text = $"⏱ {minutes:00}:{seconds:00}";
        }
    }

    /// <summary>
    /// Olum sayisini gunceller
    /// </summary>
    public void UpdateDeathCount()
    {
        if (deathCountText != null && levelManager != null)
        {
            deathCountText.text = $"Deaths: {levelManager.GetCurrentLevelDeaths()}";
        }
    }

    /// <summary>
    /// Sure dolunca cagirilir
    /// </summary>
    private void OnTimeUp()
    {
        isTimerActive = false; // Sure sayacini durdur

        Debug.Log("⏰ Sure doldu! Seviye yeniden baslatiliyor...");

        // Seviyeyi yeniden baslat
        if (levelManager != null)
        {
            levelManager.RestartCurrentLevel();
        }
    }

    /// <summary>
    /// Seviye tamamlandiktan sonra panel goster
    /// </summary>
    public void ShowLevelCompletePanel()
    {
        if (levelCompletePanel == null) return;

        // Panel'i aktif et
        levelCompletePanel.SetActive(true);

        // Mesaj yazdir
        if (levelCompleteMessageText != null)
        {
            levelCompleteMessageText.text = $"{levelManager.currentLevel.levelName}\nTamamlandi!";
        }

        // Istatistikleri yazdir
        if (levelCompleteStatsText != null)
        {
            int deaths = levelManager.GetCurrentLevelDeaths();
            string timeStr = GetFormattedTime();

            levelCompleteStatsText.text = $"Sure: {timeStr}\nOlum: {deaths}";
        }

        // Oyunu duraklat (opsiyonel)
        // Time.timeScale = 0f;
    }

    /// <summary>
    /// Level Complete Panel'i kapat ve bir sonraki seviyeye gec
    /// </summary>
    public void OnNextLevelButton()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }

        // Oyunu devam ettir
        Time.timeScale = 1f;

        // Bir sonraki seviyeyi yukle (LevelManager zaten hallediyor)
    }

    /// <summary>
    /// Formatlı sure string'i dondurur
    /// </summary>
    private string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}