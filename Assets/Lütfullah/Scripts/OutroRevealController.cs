using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OutroRevealController : MonoBehaviour
{
    [Header("UI Referansları")]
    [Tooltip("Açılmasını istediğin karartı (Image) objelerini buraya sürükle.")]
    [SerializeField] private Image[] darkSpots;

    [Header("Karartı Ayarları")]
    [Tooltip("Her bir karartının kaybolma süresi (saniye)")]
    [SerializeField] private float fadeDurationPerSpot = 1.0f;
    
    [Header("Credits Ayarları")]
    [Tooltip("Credits ekranını içeren ana UI objesi (Panel vs.)")]
    [SerializeField] private GameObject creditsPanel;
    [Tooltip("Yukarı kayacak olan yazının RectTransform'u")]
    [SerializeField] private RectTransform creditsContent;
    [Tooltip("Yazının yukarı kayma hızı")]
    [SerializeField] private float scrollSpeed = 50f;
    [Tooltip("Credits yazısının ekranda akma süresi (saniye)")]
    [SerializeField] private float creditsDuration = 10f;

    [Header("Çıkış Ayarları")]
    [Tooltip("Tüm işlemler (Credits dahil) bittikten sonra çıkmadan önce beklenecek süre")]
    [SerializeField] private float waitTimeBeforeQuit = 3.0f;

    private void Start()
    {
        // Başlangıçta Credits paneli yanlışlıkla açıksa gizleyelim
        if (creditsPanel != null) 
        {
            creditsPanel.SetActive(false);
        }

        // Sahne başladığında animasyon dizisini başlat
        StartCoroutine(RevealSequence());
    }

    private IEnumerator RevealSequence()
    {
        // 1. Karartıları sırayla yok et
        foreach (Image spot in darkSpots)
        {
            if (spot != null)
            {
                // Bir önceki karartı tamamen bitene kadar bekle
                yield return StartCoroutine(FadeOutImage(spot, fadeDurationPerSpot));
            }
        }

        // 2. Credits (Jenerik) ekranını göster ve kaydır
        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
            yield return StartCoroutine(ScrollCredits());
        }

        // 3. Her şey bittikten sonra belirlediğin süre kadar bekle
        yield return new WaitForSeconds(waitTimeBeforeQuit);
    
        // 4. Bekleme süresi bitince oyunu kapat
        QuitGame();
    }

    private IEnumerator FadeOutImage(Image img, float duration)
    {
        Color startColor = img.color;
        // Hedef renk: Mevcut rengin aynısı, sadece Alpha (saydamlık) 0
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            img.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        img.color = endColor;
    }

    private IEnumerator ScrollCredits()
    {
        float elapsedTime = 0f;

        // Belirlenen süre boyunca yazıyı yukarı doğru kaydır
        while (elapsedTime < creditsDuration)
        {
            elapsedTime += Time.deltaTime;
            
            // Atanmış bir içerik varsa Vector2.up (yukarı) yönünde hareket ettir
            if (creditsContent != null)
            {
                creditsContent.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            }

            yield return null;
        }
    }

    private void QuitGame()
    {
        Debug.Log("Oyun kapatılıyor...");
        
        // Eğer Unity Editör içerisinde test ediyorsan Play modunu durdurur
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Oyun Build alınıp EXE/APK vs. yapıldığında çalışacak gerçek kapatma komutu
        Application.Quit();
#endif
    }
}