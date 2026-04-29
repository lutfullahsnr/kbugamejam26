using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; 

public enum ControlType
{
    WASD,       
    ArrowKeys   
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
    public Vector2 groundCheckSize = new Vector2(0.4f, 0.1f);

    [Header("Hasar Ayarları")]
    public float damageCooldown = 0.5f; 

    [Header("Arayüz (UI) Ayarları")]
    public Slider healthSlider;
    public TextMeshProUGUI collectibleText; 

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;   
    
    private int currentHealth;
    private float nextDamageTime = 0f; 
    private static int collectedCount = 0;
    private static int maxCollectibles = 3; 
    private static int playersAtDoor = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ApplyStats();

        if (myStats != null)
        {
            currentHealth = myStats.maxHealth;
            
            if (healthSlider != null)
            {
                healthSlider.maxValue = myStats.maxHealth;
                healthSlider.value = currentHealth;
            }
        }
        collectedCount = 0; 
        playersAtDoor = 0;

        UpdateCollectibleUI(); 
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
                anim.SetBool("Jump", false);
            }
        }
    }

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

    private void CheckGrounded()
    {
        if (groundCheck == null) return;

        isGrounded = false;
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
        
        if (anim != null)
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

        if (isGrounded)
        {
            rb.velocity = new Vector2(horizontalInput * myStats.moveSpeed * 0.5f, rb.velocity.y);
        }
        else
        {
            rb.AddForce(new Vector2(horizontalInput * (myStats.moveSpeed * 0.25f), 0f));
            
            if (Mathf.Abs(rb.velocity.x) > myStats.moveSpeed)
            {
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * myStats.moveSpeed, rb.velocity.y);
            }
        }
    }

    private void HandleJump()
    {
        KeyCode jumpKey = (controlType == ControlType.WASD) ? KeyCode.W : KeyCode.UpArrow;

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, myStats.jumpForce);
            if (anim != null)
            {
                AudioManager.instance.ZiplamaSesiCal();
                anim.SetBool("Jump", true);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Collectible"))
        {
            Destroy(other.gameObject); 
            AudioManager.instance.ToplamaSesiCal();

            collectedCount++;
            UpdateCollectibleUI();
        }
        if (other.gameObject.CompareTag("Door"))
        {
            playersAtDoor++;
            if (collectedCount >= maxCollectibles && playersAtDoor >= 2)
            {
                GameManager.instance.ShowWinPanel();
            }
            else
            {
                Debug.Log($"Kapı kilitli! Geçmek için {maxCollectibles - collectedCount} eşya daha bulmalısın.");
            }
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            TryTakeDamage(other.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            TryTakeDamage(other.gameObject);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Door"))
        {
            playersAtDoor--; 

            if(playersAtDoor < 0) playersAtDoor = 0; 
        }
    }

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

    private void TryTakeDamage(GameObject enemyObj)
    {
        if (Time.time < nextDamageTime) return;

        Enemy enemyScript = enemyObj.GetComponent<Enemy>();

        if (enemyScript != null && enemyScript.stats != null)
        {
            currentHealth -= (int)enemyScript.stats.damage;
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
        GameManager.instance.ShowLosePanel();
    }

    private void UpdateCollectibleUI()
    {
        if (collectibleText != null)
        {
            collectibleText.text = collectedCount + "/" + maxCollectibles;
        }
    }
}