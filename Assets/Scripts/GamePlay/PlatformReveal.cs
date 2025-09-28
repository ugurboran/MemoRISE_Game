using UnityEngine;
using System.Collections; // Coroutine kullanabilmek için

// Bu script, platformun baþlangýçta görünmesini, belirli süre sonra kaybolmasýný ve opsiyonel olarak collider'ýný devre dýþý býrakmayý saðlar
[RequireComponent(typeof(SpriteRenderer))] // SpriteRenderer yoksa otomatik ekler
public class PlatformReveal : MonoBehaviour
{
    [Tooltip("Platform sahne açýldýðýnda kaç saniye görünür kalacak")]
    public float revealDuration = 5f; // Baþlangýçta görünme süresi

    [Tooltip("Alfade kaybolma süresi (saniye)")]
    public float fadeDuration = 0.4f; // Görünmez olma animasyonu süresi

    [Tooltip("Gizlenince collider kapatýlsýn mý?")]
    public bool disableColliderOnHide = false; // true olursa platform fiziksel olarak kaybolur

    SpriteRenderer sr; // Platformun görselini kontrol etmek için
    Collider2D col;   // Platformun fiziksel çarpýþmasýný kontrol etmek için
    Color origColor;  // Baþlangýç renk deðerini kaydetmek için
    bool hidden = false; // Platform gizlendi mi kontrolü

    // Awake: Component referanslarýný alýyoruz
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>(); // SpriteRenderer referansýný al
        col = GetComponent<Collider2D>();    // Collider referansýný al
        origColor = sr.color;                // Orijinal rengi kaydet
    }

    // Start: Coroutine baþlatýlýr, platformu yönetmeye baþlar
    void Start()
    {
        StartCoroutine(RevealThenHideRoutine());
    }

    // Coroutine: platform önce görünür, sonra kaybolur
    IEnumerator RevealThenHideRoutine()
    {
        // Platform baþlangýçta tamamen görünür
        sr.enabled = true;
        sr.color = new Color(origColor.r, origColor.g, origColor.b, 1f);
        if (col != null) col.enabled = true; // Collider aktif

        // Belirli süre platform görünür kalýr
        yield return new WaitForSeconds(revealDuration);

        // Fade (alfa deðeri ile yavaþça kaybolma)
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime; // geçen süreyi al
            float a = Mathf.Lerp(1f, 0f, t / fadeDuration); // alfa deðerini 1 -> 0 arasýnda interpolasyon
            sr.color = new Color(origColor.r, origColor.g, origColor.b, a); // sprite rengini güncelle
            yield return null; // bir sonraki frame'e geç
        }

        // Fade tamamlandý, platform tamamen gizlenir
        sr.enabled = false;
        if (col != null && disableColliderOnHide) col.enabled = false; // opsiyonel collider devre dýþý býrak
        hidden = true; // gizlendi olarak iþaretle
    }

    // Dýþarýdan tekrar gösterme fonksiyonu
    public void ShowAgain(float showForSeconds)
    {
        StopAllCoroutines(); // varsa eski coroutine durdur
        revealDuration = showForSeconds; // yeni süreyi ata
        StartCoroutine(RevealThenHideRoutine()); // tekrar baþlat
    }

    // Scene’de Debug için platformun gizli/aktif durumunu görselleþtirmek
    void OnDrawGizmosSelected()
    {
        if (sr != null)
        {
            Gizmos.color = hidden ? Color.red : Color.green; // gizli ise kýrmýzý, görünür ise yeþil
            Gizmos.DrawWireCube(transform.position, sr.bounds.size); // wireframe ile kutu çiz
        }
    }
}
