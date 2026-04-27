using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroRevealController : MonoBehaviour
{
    [Header("UI Referansları")]
    [Tooltip("Açılmasını istediğin karartı (Image) objelerini buraya sürükle.")]
    [SerializeField] private Image[] darkSpots;

    [Header("Ayarlar")]
    [Tooltip("Her bir karartının kaybolma süresi (saniye cinsinden)")]
    [SerializeField] private float fadeDurationPerSpot = 1.0f;
    
    [Tooltip("Geçiş yapılacak sahnenin adı")]
    [SerializeField] private string nextSceneName = "Level 1";

    private void Start()
    {
        // Sahne başladığında animasyon dizisini başlat
        StartCoroutine(RevealSequence());
    }

    private IEnumerator RevealSequence()
    {
        // Dizideki her bir Image için sırayla işlemi yap
        foreach (Image spot in darkSpots)
        {
            if (spot != null)
            {
                // Bir önceki karartı tamamen bitene kadar bekle, sonra diğerine geç
                yield return StartCoroutine(FadeOutImage(spot, fadeDurationPerSpot));
            }
        }

        // Tüm karartılar (dizi) bittikten sonra sahneye geç
        LoadNextScene();
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
            // Geçen sürenin toplam süreye oranını (0 ile 1 arası) hesapla
            float t = elapsedTime / duration;
            
            // Rengi yavaşça hedefe doğru Lerp ile kaydır
            img.color = Color.Lerp(startColor, endColor, t);
            
            // Bir sonraki frame'i bekle
            yield return null;
        }

        // Süre bittiğinde alfa değerinin tam olarak 0 olduğundan emin ol
        img.color = endColor;
    }

    private void LoadNextScene()
    {
        // Eğer geçiş yapılacak sahne Build Settings'de ekliyse geçiş yapar
        SceneManager.LoadScene(nextSceneName);
    }
}