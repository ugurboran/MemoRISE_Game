using UnityEngine;

// Bu script, kameranin oyuncuyu takip etmesini saglar
// Smooth (yumusak) gecis ile kamera oyuncuyu takip eder
public class CameraFollow : MonoBehaviour
{
    [Header("Takip Ayarlari")]
    [Tooltip("Kameranin takip edecegi obje (Player)")]
    public Transform target;

    [Tooltip("Kamera ile oyuncu arasindaki mesafe (X, Y, Z)")]
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Yumusak Gecis")]
    [Tooltip("Kamera ne kadar yumusak takip etsin (0 = hizli, 1 = yavas)")]
    [Range(0f, 1f)]
    public float smoothSpeed = 0.125f;

    [Tooltip("Yumusak gecis kullanilsin mi?")]
    public bool useSmoothFollow = true;

    [Header("Sinirlar (Opsiyonel)")]
    [Tooltip("Kamera sinirlari kullanilsin mi?")]
    public bool useBounds = false;

    [Tooltip("Kameranin gidebilecegi minimum X pozisyonu")]
    public float minX = -100f;

    [Tooltip("Kameranin gidebilecegi maksimum X pozisyonu")]
    public float maxX = 100f;

    [Tooltip("Kameranin gidebilecegi minimum Y pozisyonu")]
    public float minY = -100f;

    [Tooltip("Kameranin gidebilecegi maksimum Y pozisyonu")]
    public float maxY = 100f;

    private void LateUpdate()
    {
        // Target atanmamissa takip etme
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: Target atanmamis!");
            return;
        }

        // Hedef pozisyonu hesapla (target + offset)
        Vector3 desiredPosition = target.position + offset;

        // Sinirlar icinde mi kontrol et
        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }

        // Yumusak gecis veya aninda takip
        if (useSmoothFollow)
        {
            // Lerp ile yumusak gecis
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
        else
        {
            // Aninda takip
            transform.position = desiredPosition;
        }
    }

    // Scene'de kamera sinirlari ve takip mesafesini gorsellestir
    private void OnDrawGizmosSelected()
    {
        if (useBounds)
        {
            // Sinir cizgilerini goster (Kirmizi)
            Gizmos.color = Color.red;

            // Yatay cizgiler
            Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(maxX, minY, 0));
            Gizmos.DrawLine(new Vector3(minX, maxY, 0), new Vector3(maxX, maxY, 0));

            // Dikey cizgiler
            Gizmos.DrawLine(new Vector3(minX, minY, 0), new Vector3(minX, maxY, 0));
            Gizmos.DrawLine(new Vector3(maxX, minY, 0), new Vector3(maxX, maxY, 0));
        }

        // Target varsa, kamera ile target arasinda cizgi goster (Mavi)
        if (target != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}