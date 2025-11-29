using UnityEngine;

// Bu script, finish noktasini temsil eder
// Oyuncu bu noktaya dokunduğunda seviye tamamlanir ve LevelManager'a bildirir
[RequireComponent(typeof(Collider2D))] // Collider2D yoksa otomatik ekler
public class FinishPoint : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Oyuncu finish'e ulastiginda gosterilecek mesaj")]
    public string completionMessage = "Seviye Tamamlandi!";

    [Tooltip("Finish noktasinin rengi (Gizmos ile gosterilir)")]
    public Color gizmoColor = Color.green;

    private bool levelCompleted = false; // Seviye bir kez tamamlansin diye kontrol

    private void Awake()
    {
        // Collider'in trigger oldugundan emin ol
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true; // Trigger modda olmali
        }
    }

    // Oyuncu finish noktasina girdiginde cagirilir
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // DEBUG: Her trigger girişinde log
        Debug.Log($"[FinishPoint] Trigger Entered: {collision.gameObject.name} (Tag: {collision.tag})");

        // Player tag kontrolu
        if (collision.CompareTag("Player") && !levelCompleted)
        {
            levelCompleted = true; // Tekrar tetiklenmesini engelle

            Debug.Log($"✅ {completionMessage}");

            // LevelManager'i bul ve seviye tamamlandigini bildir
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            if (levelManager != null)
            {
                levelManager.OnLevelComplete();
            }
            else
            {
                Debug.LogError("FinishPoint: LevelManager bulunamadi!");
            }
        }
        else if (!collision.CompareTag("Player"))
        {
            Debug.Log($"[FinishPoint] Tag uyumsuz: Beklenen 'Player', gelen '{collision.tag}'");
        }
    }

    // Scene'de finish noktasini gorsellestirmek icin Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f); // Yesil daire ciz

        // Ok isareti (yukariya dogru)
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 1f);
    }
}