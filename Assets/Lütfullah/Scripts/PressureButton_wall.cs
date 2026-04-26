using UnityEngine;
using DG.Tweening; // DoTween kütüphanesini eklemeyi unutmuyoruz!

public class PressureButton_wall : MonoBehaviour
{
    [Header("Buton Ayarları")]
    [Tooltip("Butonun aşağı çökecek olan görsel objesi (Parent objeyi değil, içindeki Sprite'ı seç)")]
    public Transform buttonVisual; 
    public float buttonPressDistance = 0.05f; // Ne kadar aşağı inecek
    public float buttonAnimDuration = 0.2f;   // İnme/Çıkma hızı

    [Header("Asansör Ayarları")]
    public Transform elevator; // Asansörün kendisi
    public Transform elevatorTarget; // Asansörün gideceği hedef nokta (Boş bir GameObject)
    public float elevatorMoveDuration = 1.5f; // Asansörün hedefe varma süresi

    // ==========================================
    // YENİ EKLENEN DUVAR AYARLARI
    // ==========================================
    [Header("Gizli Duvar Ayarları")]
    [Tooltip("Tuşa basılınca X ekseninde küçülerek kaybolacak duvar")]
    public Transform secretWall; 
    public float wallHideDuration = 0.5f; // Duvarın kaybolma/gelme süresi

    // Başlangıç pozisyonlarını ve boyutlarını hafızada tutacağız
    private Vector3 buttonStartPos;
    private Vector3 elevatorStartPos;
    private Vector3 wallStartScale; // Duvarın ilk boyutunu tutacak değişken

    // Co-op için kritik: Butonun üstünde kaç kişi var?
    private int charactersOnButton = 0; 

    void Start()
    {
        // Oyun başladığında objelerin ilk yerlerini ve boyutlarını kaydet
        if (buttonVisual != null) buttonStartPos = buttonVisual.position;
        if (elevator != null) elevatorStartPos = elevator.position;
        
        // Duvarın oyun başındaki orijinal büyüklüğünü (scale) hafızaya al
        if (secretWall != null) wallStartScale = secretWall.localScale; 
    }

    // Karakter butona bastığında
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player etiketini de ekledim, ikisinden biriyse çalışır
        if (other.CompareTag("Character") || other.CompareTag("Player"))
        {
            charactersOnButton++;

            // Eğer butona basan İLK kişiyse sistemi çalıştır
            if (charactersOnButton == 1)
            {
                ActivateSystem();
            }
        }
    }

    // Karakter butondan indiğinde
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Character") || other.CompareTag("Player"))
        {
            charactersOnButton--;

            // Eğer butonda KİMSE KALMADIYSA sistemi geri al
            if (charactersOnButton <= 0)
            {
                charactersOnButton = 0; // Güvenlik önlemi
                DeactivateSystem();
            }
        }
    }

    // ==========================================
    // DOTWEEN HAREKET MANTIĞI
    // ==========================================
    
    private void ActivateSystem()
    {
        // 1. Butonu aşağı indir
        if (buttonVisual != null)
        {
            buttonVisual.DOKill();
            buttonVisual.DOMoveY(buttonStartPos.y - buttonPressDistance, buttonAnimDuration);
        }

        // 2. Asansörü hedefe götür
        if (elevator != null && elevatorTarget != null)
        {
            elevator.DOKill();
            elevator.DOMove(elevatorTarget.position, elevatorMoveDuration).SetEase(Ease.InOutSine);
        }

        // 3. Duvarı X ekseninde küçülterek yok et (Sürgülü kapı gibi açılır)
        if (secretWall != null)
        {
            secretWall.DOKill();
            // X ekseni scale'ini 0 yapıyoruz, fiziksel olarak da collider küçüleceği için yollar açılır
            secretWall.DOScaleX(0f, wallHideDuration).SetEase(Ease.InOutSine);
        }
    }

    private void DeactivateSystem()
    {
        // 1. Butonu eski (yukarı) haline getir
        if (buttonVisual != null)
        {
            buttonVisual.DOKill();
            buttonVisual.DOMoveY(buttonStartPos.y, buttonAnimDuration);
        }

        // 2. Asansörü başlangıç noktasına geri getir
        if (elevator != null)
        {
            elevator.DOKill();
            elevator.DOMove(elevatorStartPos, elevatorMoveDuration).SetEase(Ease.InOutSine);
        }

        // 3. Duvarı orijinal haline geri döndür (Kapanma)
        if (secretWall != null)
        {
            secretWall.DOKill();
            // Hafızaya aldığımız orijinal X scale değerine geri dönüyor
            secretWall.DOScaleX(wallStartScale.x, wallHideDuration).SetEase(Ease.InOutSine);
        }
    }
}