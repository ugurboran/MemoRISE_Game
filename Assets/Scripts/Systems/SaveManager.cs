using UnityEngine;

// Oyun verilerini kaydetme ve yukleme islemlerini yoneten merkezi sistem
// PlayerPrefs kullanarak basit verileri kaydeder
public static class SaveManager
{
    // Kayit anahtarlari (key'ler)
    private const string CURRENT_LEVEL_INDEX = "CurrentLevelIndex";
    private const string CURRENT_LEVEL_DEATH_COUNT = "CurrentLevelDeathCount";
    private const string TOTAL_DEATH_COUNT = "TotalDeathCount";
    private const string MASTER_VOLUME = "MasterVolume";
    private const string SFX_VOLUME = "SFXVolume";

    // === SEVIYE YONETIMI ===

    /// <summary>
    /// Mevcut seviye index'ini kaydeder
    /// </summary>
    public static void SaveCurrentLevel(int levelIndex)
    {
        PlayerPrefs.SetInt(CURRENT_LEVEL_INDEX, levelIndex);
        PlayerPrefs.Save();
        Debug.Log($"[SaveManager] Seviye kaydedildi: Level {levelIndex + 1}");
    }

    /// <summary>
    /// Kaydedilen seviye index'ini yukler (varsayilan: 0)
    /// </summary>
    public static int LoadCurrentLevel()
    {
        return PlayerPrefs.GetInt(CURRENT_LEVEL_INDEX, 0);
    }

    /// <summary>
    /// Seviye ilerlemesini sifirlar (yeni oyun)
    /// </summary>
    public static void ResetLevelProgress()
    {
        PlayerPrefs.SetInt(CURRENT_LEVEL_INDEX, 0);
        PlayerPrefs.Save();
        Debug.Log("[SaveManager] Seviye ilerlemesi sifirlandi.");
    }

    // === OLUM SAYACI ===

    /// <summary>
    /// Mevcut seviyedeki olum sayisini kaydeder
    /// </summary>
    public static void SaveCurrentLevelDeaths(int deaths)
    {
        PlayerPrefs.SetInt(CURRENT_LEVEL_DEATH_COUNT, deaths);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Mevcut seviyedeki olum sayisini yukler
    /// </summary>
    public static int LoadCurrentLevelDeaths()
    {
        return PlayerPrefs.GetInt(CURRENT_LEVEL_DEATH_COUNT, 0);
    }

    /// <summary>
    /// Toplam olum sayisini kaydeder
    /// </summary>
    public static void SaveTotalDeaths(int deaths)
    {
        PlayerPrefs.SetInt(TOTAL_DEATH_COUNT, deaths);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Toplam olum sayisini yukler
    /// </summary>
    public static int LoadTotalDeaths()
    {
        return PlayerPrefs.GetInt(TOTAL_DEATH_COUNT, 0);
    }

    /// <summary>
    /// Olum sayaclarini sifirla
    /// </summary>
    public static void ResetDeathCounts()
    {
        PlayerPrefs.SetInt(CURRENT_LEVEL_DEATH_COUNT, 0);
        PlayerPrefs.SetInt(TOTAL_DEATH_COUNT, 0);
        PlayerPrefs.Save();
        Debug.Log("[SaveManager] Olum sayaclari sifirlandi.");
    }

    // === SES AYARLARI ===

    /// <summary>
    /// Master ses seviyesini kaydeder (0-1 arasi)
    /// </summary>
    public static void SaveMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME, volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Master ses seviyesini yukler (varsayilan: 0.8)
    /// </summary>
    public static float LoadMasterVolume()
    {
        return PlayerPrefs.GetFloat(MASTER_VOLUME, 0.8f);
    }

    /// <summary>
    /// SFX ses seviyesini kaydeder (0-1 arasi)
    /// </summary>
    public static void SaveSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat(SFX_VOLUME, volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// SFX ses seviyesini yukler (varsayilan: 1.0)
    /// </summary>
    public static float LoadSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFX_VOLUME, 1.0f);
    }

    // === GENEL ===

    /// <summary>
    /// Tum kayitlari siler (oyunu sifirla)
    /// </summary>
    public static void DeleteAllSaveData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[SaveManager] TUM KAYITLAR SILINDI!");
    }

    /// <summary>
    /// Kayitlarin olup olmadigini kontrol eder
    /// </summary>
    public static bool HasSaveData()
    {
        return PlayerPrefs.HasKey(CURRENT_LEVEL_INDEX);
    }
}