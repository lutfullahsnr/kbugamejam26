using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    // Radius (Yarıçap) yerine Size (Boyut) kullanacağız
    public Vector2 groundCheckSize = new Vector2(0.4f, 0.1f);

    [Header("Hasar Ayarları")]
    [Tooltip("Hasar aldıktan sonra saniye cinsinden yenilmezlik süresi")]
    public float damageCooldown = 0.5f; 

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;   
    
    // Anlık can değerini ScriptableObject'in bozulmaması için burada tutuyoruz
    private int currentHealth;
    // Sürekli hasar yemesini (lav vb.) engellemek için zamanlayıcı
    private float nextDamageTime = 0f; 

    public Slider healthSlider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim=GetComponent<Animator>();
        ApplyStats();

        // Oyun başladığında canı ScriptableObject'teki maksimum cana eşitle
        if (myStats != null)
        {
            currentHealth = myStats.maxHealth;
            
            // Slider ayarlarını başlangıçta maksimum cana göre uyarla
            if (healthSlider != null)
            {
                healthSlider.maxValue = myStats.maxHealth;
                healthSlider.value = currentHealth;
            }
        }
    }

    void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleJump();

        if (anim != null)
        {
            if(isGrounded && rb.velocity.y <= 0.1f)
            {
                anim.SetBool("Jump",false);
            }
        }
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
    /// GroundCheck: Karakterin ayaklarının altında ince bir dikdörtgen oluşturarak zemin arar.
    /// Yan duvarlara sürtünürken zıplama bug'ını önler.
    /// </summary>
    private void CheckGrounded()
    {
        if (groundCheck == null) return;

        isGrounded = false;

        // OverlapCircleAll yerine OverlapBoxAll kullanıyoruz (Açı = 0f)
        Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheck.position, groundCheckSize, 0f, groundLayer);

        foreach (Collider2D col in colliders)
        {
            if (col.gameObject != gameObject)
            {
                isGrounded = true;
                break; 
            }
        }
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
        if (anim!= null)
        {
            anim.SetFloat("Speed", Mathf.Abs(horizontalInput));
        }
        if (horizontalInput < 0)
        {
            Vector3 scaler = transform.localScale;
            scaler.x = -Mathf.Abs(scaler.x);
            transform.localScale = scaler;
        }
        if (horizontalInput > 0)
        {
            Vector3 scaler = transform.localScale;
            scaler.x = Mathf.Abs(scaler.x);
            transform.localScale = scaler;
        }

        // Havadayken duvara/karaktere yapışmayı önleyen asıl kod değişikliği:
        if (isGrounded)
        {
            // Yerdeyken normal hızımızı anında uyguluyoruz
            rb.velocity = new Vector2(horizontalInput * myStats.moveSpeed *0.5f, rb.velocity.y);
        }
        else
        {
            // HAVADAYKEN: Fizik motorunun gücünü (AddForce) kullanıyoruz. 
            // Böylece zorla duvara girip yapışmak yerine, çarptığında doğal bir şekilde aşağı kayıyor.
            rb.AddForce(new Vector2(horizontalInput * (myStats.moveSpeed * 0.25f), 0f));
            
            // Havada aşırı hızlanmayı engellemek için maksimum hızı sınırla
            if (Mathf.Abs(rb.velocity.x) > myStats.moveSpeed)
            {
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * myStats.moveSpeed, rb.velocity.y);
            }
        }
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
            if (anim != null)
            {
                anim.SetBool("Jump", true);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        
        Gizmos.color = isGrounded ? Color.green : Color.red;
        // DrawWireSphere yerine DrawWireCube kullanıyoruz
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
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

            if (healthSlider != null)
            {
                healthSlider.value = currentHealth;
            }

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