using UnityEngine;

// PlayerController - Unity6+ uyumlu (Rigidbody2D.linearVelocity kullan�r)
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarlar�")]
    public float moveSpeed = 3f;  // yatay h�z (birim/s)
    public float jumpForce = 2f; // z�plama i�in impulse (veya do�rudan velocity atamak istersen farkl� de�er)

    [Header("Zemin Kontrol")]
    public LayerMask groundLayer;        // platformlar�n layer'�
    public Transform groundCheck;        // ayak pozisyonu (OverlapCircle i�in)
    public float groundCheckRadius = 0.12f;

    Rigidbody2D rb;
    float horiz;       // -1 .. 1 yatay giri�
    bool jumpPressed;  // z�plama inputu

    void Awake()
    {
        // Rigidbody2D referans�n� al�yoruz: tekrar tekrar GetComponent �a��rmamak i�in sakla
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Girdi alma - Update i�inde oku (FixedUpdate i�inde fizik uygula)
        horiz = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
            jumpPressed = true;

        // Basit mobil test: ekrana sola/sa�a dokununca hareket et
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began || t.phase == TouchPhase.Stationary || t.phase == TouchPhase.Moved)
            {
                horiz = (t.position.x < Screen.width / 2) ? -1 : 1;
            }
        }
    }

    void FixedUpdate()
    {
        // **�NEML�:** linearVelocity do�rudan rb.velocity yerine kullan�l�yor.
        // �lk olarak mevcut linear velocity'i al, sonra sadece X de�erini de�i�tir.
        // Bu, Y y�n�ndeki d��me/z�plama h�z�n� bozmadan yatay kontrol sa�lar.
        Vector2 lv = rb.linearVelocity; // rb.linearVelocity d�nd�r�lebilir Vector2

        // E�er Rigidbody tipi 'Kinematic' ise linearVelocity atamak desteklenmeyebilir.
        // Kinematic ise alternative hareket y�ntemi kullan (transform veya MovePosition).
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            // Basit fallback: transform ile hareket (kullan�c� iste�ine g�re adapte et)
            transform.position += new Vector3(horiz * moveSpeed * Time.fixedDeltaTime, 0f, 0f);
        }
        else
        {
            // Dynamic veya Static de�ilse: X komponentini set et
            lv.x = horiz * moveSpeed;
            rb.linearVelocity = lv; // burada linearVelocity'i tekrar set ediyoruz
            // Alternatif daha k�sa: rb.linearVelocityX = horiz * moveSpeed;
        }

        // Z�plama
        if (jumpPressed && IsGrounded())
        {
            // Se�enek A (daha "ani" kontrol): vertical velocity'i s�f�rla ve impulse uygula
            // Bu, �nceki kodunda oldu�u gibi davran��� korur:
            lv = rb.linearVelocity;
            lv.y = 0f;           // mevcut d���� h�z�n� temizle (iste�e ba�l�)
            rb.linearVelocity = lv;

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // anl�k z�plama

            // Se�enek B (do�rudan h�z atama - AddForce istemezsen):
            // rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeedValue);

            jumpPressed = false;
        }
    }

    // GroundCheck ile zemin kontrol�
    bool IsGrounded()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
