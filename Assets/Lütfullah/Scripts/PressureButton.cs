using UnityEngine;
using DG.Tweening; // DoTween kütüphanesini eklemeyi unutmuyoruz!

public class PressureButton : MonoBehaviour
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

    // Başlangıç pozisyonlarını hafızada tutacağız
    private Vector3 buttonStartPos;
    private Vector3 elevatorStartPos;

    // Co-op için kritik: Butonun üstünde kaç kişi var?
    private int charactersOnButton = 0; 

    void Start()
    {
        // Oyun başladığında objelerin ilk yerlerini kaydet
        if (buttonVisual != null) buttonStartPos = buttonVisual.position;
        if (elevator != null) elevatorStartPos = elevator.position;
    }

    // Karakter butona bastığında
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Character"))
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
        // DOKill() eski hareketleri iptal edip çakışmayı önler
        buttonVisual.DOKill();
        buttonVisual.DOMoveY(buttonStartPos.y - buttonPressDistance, buttonAnimDuration);

        // 2. Asansörü hedefe götür
        if (elevator != null && elevatorTarget != null)
        {
            elevator.DOKill();
            // SetEase(Ease.InOutSine) asansörün kalkışta ve duruşta yumuşak ivmelenmesini sağlar
            elevator.DOMove(elevatorTarget.position, elevatorMoveDuration).SetEase(Ease.InOutSine);
        }
    }

    private void DeactivateSystem()
    {
        // 1. Butonu eski (yukarı) haline getir
        buttonVisual.DOKill();
        buttonVisual.DOMoveY(buttonStartPos.y, buttonAnimDuration);

        // 2. Asansörü başlangıç noktasına geri getir
        if (elevator != null)
        {
            elevator.DOKill();
            elevator.DOMove(elevatorStartPos, elevatorMoveDuration).SetEase(Ease.InOutSine);
        }
    }
}