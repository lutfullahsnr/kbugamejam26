using System.Collections.Generic;
using UnityEngine;

public class SmartWind : MonoBehaviour
{
    [Header("Rüzgar Ayarları")]
    public float windForce = 50f; 
    // Rüzgar yönü otomatik olarak bu objenin "Sağ" (X) eksenidir. 
    // Yani fan objesini döndürdüğünde rüzgar da oraya eser!

    [Header("Engeller (Siperler)")]
    [Tooltip("Rüzgarı hangi katmanlar kesebilir? (Player ve Ground kesinlikle seçili olmalı)")]
    public LayerMask obstacleLayer;

    // Rüzgar alanına girenleri tutacağımız liste
    private List<Rigidbody2D> objectsInWind = new List<Rigidbody2D>();

    void OnTriggerEnter2D(Collider2D other)
    {
        // Alana giren objenin Rigidbody'si varsa listeye ekle
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            objectsInWind.Add(rb);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Alandan çıkarsa listeden çıkar
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null && objectsInWind.Contains(rb))
        {
            objectsInWind.Remove(rb);
        }
    }

    void FixedUpdate()
    {
        // Listedeki herkes için rüzgar hesabı yap
        for (int i = objectsInWind.Count - 1; i >= 0; i--)
        {
            Rigidbody2D rb = objectsInWind[i];

            if (rb == null) 
            {
                objectsInWind.RemoveAt(i);
                continue;
            }

            // Fanın merkezinden, objeye doğru yönü ve mesafeyi hesapla
            Vector2 directionToObject = rb.transform.position - transform.position;
            float distance = directionToObject.magnitude;

            // Fandan objeye doğru bir görünmez ışın (Raycast) yolla
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToObject.normalized, distance, obstacleLayer);

            // Eğer ışın hiçbir şeye çarpmadan veya DOĞRUDAN bu objeye çarparak ulaştıysa (Arada siper yoksa!)
            if (hit.collider == null || hit.collider.gameObject == rb.gameObject)
            {
                // Fiziksel gücü uygula! (Unity'nin fizik motoru, kütlesi [mass] yüksek olan babayı otomatik olarak daha az itecektir)
                rb.AddForce(transform.right * windForce);
            }
        }
    }
    // ==========================================
    // EDİTÖR GÖRSELLEŞTİRMESİ (GİZMO)
    // ==========================================
    private void OnDrawGizmos()
    {
        // Rüzgarın merkezini ve yönünü al (transform.right)
        Vector3 startPos = transform.position;
        Vector3 windDirection = transform.right;
        
        // Görsel okun uzunluğunu belirle
        float arrowLength = 2.5f; 
        Vector3 endPos = startPos + (windDirection * arrowLength);

        // Rüzgar hissiyatı vermek için rengi açık mavi (Cyan) yapıyoruz
        Gizmos.color = Color.cyan;

        // Ana doğrultu çizgisini çiz
        Gizmos.DrawLine(startPos, endPos);

        // Okun "V" şeklindeki uç kısımlarını hesapla ve çiz
        Vector3 arrowTip1 = endPos - (windDirection * 0.5f) + (transform.up * 0.5f);
        Vector3 arrowTip2 = endPos - (windDirection * 0.5f) - (transform.up * 0.5f);

        Gizmos.DrawLine(endPos, arrowTip1);
        Gizmos.DrawLine(endPos, arrowTip2);
    }
}