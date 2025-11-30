using UnityEngine;

// Bu script, oyuncunun belirli bir Y seviyesinin altina dusmesini kontrol eder
// Eger oyuncu death zone'a girerse, seviye yeniden baslatilir
public class DeathZone : MonoBehaviour
{
    [Header("Death Zone Ayarlari")]
    [Tooltip("Oyuncu bu Y pozisyonunun altina dusmesi durumunda olur")]
    public float deathY = -10f;

    [Tooltip("Death zone rengi (Gizmos ile gosterilir)")]
    public Color gizmoColor = Color.red;

    [Header("Debug")]
    [Tooltip("Console'da olum mesajlari goster")]
    public bool showDebugLogs = true;

    private Transform playerTransform; // Oyuncu referansi
    private bool playerDied = false; // Bir kere olum kontrolu

    private void Start()
    {
        // Player'i bul ve referansini tut
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("DeathZone: Player bulunamadi! 'Player' tag'i atanmis mi?");
        }
    }

    private void Update()
    {
        // Player yoksa veya zaten olduyse kontrol etme
        if (playerTransform == null || playerDied) return;

        // Oyuncu death zone'un altina dustu mu kontrol et
        if (playerTransform.position.y < deathY)
        {
            OnPlayerDeath();
        }
    }

    /// <summary>
    /// Oyuncu olunce cagrilir
    /// </summary>
    private void OnPlayerDeath()
    {
        playerDied = true; // Tekrar cagirilmasini engelle

        if (showDebugLogs)
        {
            Debug.Log($"💀 Oyuncu death zone'a dustu! (Y: {playerTransform.position.y})");
        }

        // LevelManager'i bul ve seviyeyi yeniden baslat
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.RestartCurrentLevel();
        }
        else
        {
            Debug.LogError("DeathZone: LevelManager bulunamadi!");
        }
    }

    /// <summary>
    /// Yeni seviye yuklendiginde death zone'u sifirla
    /// </summary>
    public void ResetDeathZone()
    {
        playerDied = false;

        // Player referansini yeniden al (yeni seviyede spawn olabilir)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        if (showDebugLogs)
        {
            Debug.Log("[DeathZone] Reset edildi.");
        }
    }

    // Scene'de death zone cizgisini goster (kirmizi)
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // Yatay cizgi (death zone seviyesi)
        float lineLength = 100f; // Uzun bir cizgi
        Vector3 leftPoint = new Vector3(-lineLength, deathY, 0);
        Vector3 rightPoint = new Vector3(lineLength, deathY, 0);

        Gizmos.DrawLine(leftPoint, rightPoint);

        // Her 10 birimde bir dikey cizgi (izgara gorunumu)
        for (float x = -lineLength; x <= lineLength; x += 10f)
        {
            Gizmos.DrawLine(new Vector3(x, deathY, 0), new Vector3(x, deathY - 1f, 0));
        }
    }
}