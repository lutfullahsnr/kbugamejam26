using UnityEngine;
using UnityEngine.SceneManagement;

public enum ControlType
{
    WASD,       // Karakter 1: W/A/D tuşları
    ArrowKeys   // Karakter 2: Yön tuşları
}

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Karakter İstatistikleri")]
    public CharacterStats myStats; 

    [Header("Kontrol Şeması")]
    public ControlType controlType = ControlType.WASD; 

    [Header("Zemin Kontrolü")]
    public Transform groundCheck;           
    public LayerMask groundLayer;           
    public float groundCheckRadius = 0.15f; 

    [Header("Hasar Ayarları")]
    [Tooltip("Hasar aldıktan sonra saniye cinsinden yenilmezlik süresi")]
    public float damageCooldown = 0.5f; 

    private Rigidbody2D rb;
    private bool isGrounded;   
    
    // Anlık can değerini ScriptableObject'in bozulmaması için burada tutuyoruz
    private int currentHealth;
    // Sürekli hasar yemesini (lav vb.) engellemek için zamanlayıcı
    private float nextDamageTime = 0f; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ApplyStats();

        // Oyun başladığında canı ScriptableObject'teki maksimum cana eşitle
        if (myStats != null)
        {
            currentHealth = myStats.maxHealth;
        }
    }

    void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleJump();
    }

    /// <summary>
    /// CharacterStats'tan alınan değerleri Rigidbody2D bileşenine uygular.
    /// </summary>
    public void ApplyStats()
    {
        if (myStats == null)
        {
            Debug.LogWarning($"{gameObject.name}: CharacterStats atanmamış!", this);
            return;
        }

        rb = rb != null ? rb : GetComponent<Rigidbody2D>();
        rb.mass         = myStats.mass;
        rb.gravityScale = myStats.gravityScale;
    }

    /// <summary>
    /// GroundCheck: Karakterin yerde olup olmadığını kontrol eder.
    /// </summary>
    private void CheckGrounded()
    {
        if (groundCheck == null) return;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    /// <summary>
    /// Hareket: Karakterin yatay hareketini kontrol eder. 
    /// </summary>
    private void HandleMovement()
    {
        float horizontalInput = 0f;

        if (controlType == ControlType.WASD)
        {
            if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
            if (Input.GetKey(KeyCode.D)) horizontalInput =  1f;
        }
        else 
        {
            if (Input.GetKey(KeyCode.LeftArrow))  horizontalInput = -1f;
            if (Input.GetKey(KeyCode.RightArrow)) horizontalInput =  1f;
        }

        rb.velocity = new Vector2(horizontalInput * myStats.moveSpeed, rb.velocity.y);
    }

    /// <summary>
    /// Zıplama: Karakterin zıplamasını kontrol eder.
    /// </summary>
    private void HandleJump()
    {
        KeyCode jumpKey = (controlType == ControlType.WASD) ? KeyCode.W : KeyCode.UpArrow;

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, myStats.jumpForce);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    // ==========================================
    // ÇARPIŞMA VE ETKİLEŞİM (KAPI, ALTIN, DÜŞMAN)
    // ==========================================

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Collectible"))
        {
            Destroy(other.gameObject); 
        }

        if (other.gameObject.CompareTag("Door"))
        {
            if(SceneManager.GetActiveScene().name == "Level 3")
                SceneManager.LoadScene("LastScene");
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        // Düşman "Trigger" alanına sahipse (Örn: Lazer, Ateş topu)
        if (other.gameObject.CompareTag("Enemy"))
        {
            TryTakeDamage(other.gameObject);
        }
    }

    // Düşmanın (Örn: Lavın) içinde durdukça sürekli çalışacak olan fonksiyon
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            TryTakeDamage(other.gameObject);
        }
    }

    // Eğer düşman fiziksel olarak katıysa (Is Trigger kapalıysa) Enter ve Stay versiyonları
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TryTakeDamage(collision.gameObject);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TryTakeDamage(collision.gameObject);
        }
    }

    // ==========================================
    // HASAR ALMA VE ÖLÜM SİSTEMİ
    // ==========================================

    private void TryTakeDamage(GameObject enemyObj)
    {
        // Karakter şu anda yenilmezlik süresindeyse (cooldown bitmediyse) hasar alma
        if (Time.time < nextDamageTime) return;

        // Çarptığımız düşmanın üzerindeki "Enemy" scriptini alıyoruz
        Enemy enemyScript = enemyObj.GetComponent<Enemy>();

        // Eğer objede script varsa ve stats atandıysa
        if (enemyScript != null && enemyScript.stats != null)
        {
            // EnemyStats içindeki float hasarı int'e çevirip kendi canımızdan düşüyoruz
            currentHealth -= (int)enemyScript.stats.damage;
            
            // Bekleme süresini ileriye at (Örn: 0.5 saniye boyunca tekrar hasar yiyemez)
            nextDamageTime = Time.time + damageCooldown;

            Debug.Log($"{gameObject.name} hasar aldı! Kalan Can: {currentHealth}");

            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} öldü! Bölüm Yeniden Başlatılıyor.");
        // Karakter ölünce şimdilik bulunduğu bölümü baştan başlatır
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}