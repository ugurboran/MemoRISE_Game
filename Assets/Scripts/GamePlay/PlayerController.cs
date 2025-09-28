using UnityEngine;

// PlayerController - Unity6+ uyumlu (Rigidbody2D.linearVelocity kullanýr)
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float moveSpeed = 3f;  // yatay hýz (birim/s)
    public float jumpForce = 2f; // zýplama için impulse (veya doðrudan velocity atamak istersen farklý deðer)

    [Header("Zemin Kontrol")]
    public LayerMask groundLayer;        // platformlarýn layer'ý
    public Transform groundCheck;        // ayak pozisyonu (OverlapCircle için)
    public float groundCheckRadius = 0.12f;

    Rigidbody2D rb;
    float horiz;       // -1 .. 1 yatay giriþ
    bool jumpPressed;  // zýplama inputu

    void Awake()
    {
        // Rigidbody2D referansýný alýyoruz: tekrar tekrar GetComponent çaðýrmamak için sakla
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Girdi alma - Update içinde oku (FixedUpdate içinde fizik uygula)
        horiz = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
            jumpPressed = true;

        // Basit mobil test: ekrana sola/saða dokununca hareket et
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
        // **ÖNEMLÝ:** linearVelocity doðrudan rb.velocity yerine kullanýlýyor.
        // Ýlk olarak mevcut linear velocity'i al, sonra sadece X deðerini deðiþtir.
        // Bu, Y yönündeki düþme/zýplama hýzýný bozmadan yatay kontrol saðlar.
        Vector2 lv = rb.linearVelocity; // rb.linearVelocity döndürülebilir Vector2

        // Eðer Rigidbody tipi 'Kinematic' ise linearVelocity atamak desteklenmeyebilir.
        // Kinematic ise alternative hareket yöntemi kullan (transform veya MovePosition).
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            // Basit fallback: transform ile hareket (kullanýcý isteðine göre adapte et)
            transform.position += new Vector3(horiz * moveSpeed * Time.fixedDeltaTime, 0f, 0f);
        }
        else
        {
            // Dynamic veya Static deðilse: X komponentini set et
            lv.x = horiz * moveSpeed;
            rb.linearVelocity = lv; // burada linearVelocity'i tekrar set ediyoruz
            // Alternatif daha kýsa: rb.linearVelocityX = horiz * moveSpeed;
        }

        // Zýplama
        if (jumpPressed && IsGrounded())
        {
            // Seçenek A (daha "ani" kontrol): vertical velocity'i sýfýrla ve impulse uygula
            // Bu, önceki kodunda olduðu gibi davranýþý korur:
            lv = rb.linearVelocity;
            lv.y = 0f;           // mevcut düþüþ hýzýný temizle (isteðe baðlý)
            rb.linearVelocity = lv;

            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // anlýk zýplama

            // Seçenek B (doðrudan hýz atama - AddForce istemezsen):
            // rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeedValue);

            jumpPressed = false;
        }
    }

    // GroundCheck ile zemin kontrolü
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
