using UnityEngine;

// Bu ScriptableObject, her seviye için gerekli bilgileri tutar
// Inspector'dan kolayca düzenlenebilir seviye veri dosyaları oluşturmamızı sağlar
[CreateAssetMenu(fileName = "Level_01", menuName = "MemoRISE/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    [Header("Seviye Bilgileri")]
    [Tooltip("Seviyenin adı (örn: Level 1, Tutorial)")]
    public string levelName = "Level 1";

    [Tooltip("Seviye numarası")]
    public int levelNumber = 1;

    [Header("Platform Ayarları")]
    [Tooltip("Bu seviyede kaç platform olacak")]
    public int platformCount = 5;

    [Tooltip("Platformlar kaç saniye görünür kalacak")]
    public float revealDuration = 5f;

    [Tooltip("İLK platform Player'dan ne kadar uzakta olacak (zıplanabilir mesafe)")]
    public float firstPlatformDistance = 2f; // BUNU EKLE

    [Tooltip("Platformlar arası minimum mesafe (zıplanabilir)")]
    public float minJumpDistance = 2f;

    [Tooltip("Platformlar arası maksimum mesafe (zıplanabilir)")]
    public float maxJumpDistance = 4f;

    [Tooltip("Y ekseninde maksimum yükseklik farkı")]
    public float maxHeightVariation = 2f;

    [Tooltip("Platform yönü (true = sağa, false = yukarı)")]
    public bool horizontalPath = true;

    /*
     * 
    // Silinecek alan ? 

    [Header("Alan Sınırları")]
    [Tooltip("Platformların oluşturulacağı X ekseni minimum değeri")]
    public float minX = 0f;

    [Tooltip("Platformların oluşturulacağı X ekseni maksimum değeri")]
    public float maxX = 20f;

    [Tooltip("Platformların oluşturulacağı Y ekseni minimum değeri")]
    public float minY = -3f;

    [Tooltip("Platformların oluşturulacağı Y ekseni maksimum değeri")]
    public float maxY = 3f;

    */

    [Header("Zorluk Ayarları")]
    [Tooltip("Bu seviyeyi tamamlamak için süre limiti var mı?")]
    public bool hasTimeLimit = false;

    [Tooltip("Süre limiti (saniye)")]
    public float timeLimit = 60f;

    [Tooltip("İlk platformun başlangıç pozisyonu (World space)")]
    public Vector2 firstPlatformPosition = new Vector2(0f, 0f);

    [Tooltip("Finish Point son platformdan ne kadar uzakta olacak (X, Y offset)")]
    public Vector2 finishOffsetFromLastPlatform = new Vector2(0f, 0f);
}