using UnityEngine;

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
    private Rigidbody2D rb;
    private bool isGrounded;   

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ApplyStats();
    }

    void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleJump();
    }
    /// <summary>
    /// CharacterStats'tan alınan değerleri Rigidbody2D bileşenine uygular.
    /// </summary> <summary>
    /// 
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
    /// GroundCheck: Karakterin yerde olup olmadığını kontrol eder. GroundCheck noktası etrafında daire şeklinde bir sorgu yaparak, zemine temas edip etmediğini belirler.
    /// </summary> <summary>
    /// 
    /// </summary>
    private void CheckGrounded()
    {
        if (groundCheck == null) return;

        // Physics2D.OverlapCircle: groundCheck etrafında daire şeklinde sorgu yapar
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
    /// <summary>
    /// Hareket: Karakterin yatay hareketini kontrol eder. Kontrol tipine göre hangi tuşların hareketi tetiklediğini belirler ve 
    /// Rigidbody2D'nin velocity'sini günceller. Y hızını koruyarak sadece X ekseninde hareket sağlar. 
    /// </summary> <summary>
    /// 
    /// </summary>
    private void HandleMovement()
    {
        float horizontalInput = 0f;

        if (controlType == ControlType.WASD)
        {
            // A = -1, D = +1
            if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
            if (Input.GetKey(KeyCode.D)) horizontalInput =  1f;
        }
        else // ArrowKeys
        {
            // Sol Ok = -1, Sağ Ok = +1
            if (Input.GetKey(KeyCode.LeftArrow))  horizontalInput = -1f;
            if (Input.GetKey(KeyCode.RightArrow)) horizontalInput =  1f;
        }

        // Y hızını koruyarak sadece X eksenini güncelle
        rb.velocity = new Vector2(horizontalInput * myStats.moveSpeed, rb.velocity.y);
    }
    /// <summary>
    /// Zıplama: Karakterin zıplamasını kontrol eder. Kontrol tipine göre hangi tuşun zıplamayı tetiklediğini belirler.
    /// </summary>
    private void HandleJump()
    {
        // Zıplama tuşunu kontrol tipine göre belirle
        KeyCode jumpKey = (controlType == ControlType.WASD) ? KeyCode.W : KeyCode.UpArrow;

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            // Dikey hızı doğrudan jumpForce değerine ayarla
            rb.velocity = new Vector2(rb.velocity.x, myStats.jumpForce);
        }
    }
    /// <summary>
    /// OnDrawGizmosSelected: Unity editöründe, groundCheck noktası etrafında bir daire çizerek karakterin yerde olup olmadığını görsel olarak gösterir.
    /// </summary> <summary>
    /// 
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}