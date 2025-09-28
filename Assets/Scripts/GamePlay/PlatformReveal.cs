using UnityEngine;
using System.Collections; // Coroutine kullanabilmek i�in

// Bu script, platformun ba�lang��ta g�r�nmesini, belirli s�re sonra kaybolmas�n� ve opsiyonel olarak collider'�n� devre d��� b�rakmay� sa�lar
[RequireComponent(typeof(SpriteRenderer))] // SpriteRenderer yoksa otomatik ekler
public class PlatformReveal : MonoBehaviour
{
    [Tooltip("Platform sahne a��ld���nda ka� saniye g�r�n�r kalacak")]
    public float revealDuration = 5f; // Ba�lang��ta g�r�nme s�resi

    [Tooltip("Alfade kaybolma s�resi (saniye)")]
    public float fadeDuration = 0.4f; // G�r�nmez olma animasyonu s�resi

    [Tooltip("Gizlenince collider kapat�ls�n m�?")]
    public bool disableColliderOnHide = false; // true olursa platform fiziksel olarak kaybolur

    SpriteRenderer sr; // Platformun g�rselini kontrol etmek i�in
    Collider2D col;   // Platformun fiziksel �arp��mas�n� kontrol etmek i�in
    Color origColor;  // Ba�lang�� renk de�erini kaydetmek i�in
    bool hidden = false; // Platform gizlendi mi kontrol�

    // Awake: Component referanslar�n� al�yoruz
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>(); // SpriteRenderer referans�n� al
        col = GetComponent<Collider2D>();    // Collider referans�n� al
        origColor = sr.color;                // Orijinal rengi kaydet
    }

    // Start: Coroutine ba�lat�l�r, platformu y�netmeye ba�lar
    void Start()
    {
        StartCoroutine(RevealThenHideRoutine());
    }

    // Coroutine: platform �nce g�r�n�r, sonra kaybolur
    IEnumerator RevealThenHideRoutine()
    {
        // Platform ba�lang��ta tamamen g�r�n�r
        sr.enabled = true;
        sr.color = new Color(origColor.r, origColor.g, origColor.b, 1f);
        if (col != null) col.enabled = true; // Collider aktif

        // Belirli s�re platform g�r�n�r kal�r
        yield return new WaitForSeconds(revealDuration);

        // Fade (alfa de�eri ile yava��a kaybolma)
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime; // ge�en s�reyi al
            float a = Mathf.Lerp(1f, 0f, t / fadeDuration); // alfa de�erini 1 -> 0 aras�nda interpolasyon
            sr.color = new Color(origColor.r, origColor.g, origColor.b, a); // sprite rengini g�ncelle
            yield return null; // bir sonraki frame'e ge�
        }

        // Fade tamamland�, platform tamamen gizlenir
        sr.enabled = false;
        if (col != null && disableColliderOnHide) col.enabled = false; // opsiyonel collider devre d��� b�rak
        hidden = true; // gizlendi olarak i�aretle
    }

    // D��ar�dan tekrar g�sterme fonksiyonu
    public void ShowAgain(float showForSeconds)
    {
        StopAllCoroutines(); // varsa eski coroutine durdur
        revealDuration = showForSeconds; // yeni s�reyi ata
        StartCoroutine(RevealThenHideRoutine()); // tekrar ba�lat
    }

    // Scene�de Debug i�in platformun gizli/aktif durumunu g�rselle�tirmek
    void OnDrawGizmosSelected()
    {
        if (sr != null)
        {
            Gizmos.color = hidden ? Color.red : Color.green; // gizli ise k�rm�z�, g�r�n�r ise ye�il
            Gizmos.DrawWireCube(transform.position, sr.bounds.size); // wireframe ile kutu �iz
        }
    }
}
