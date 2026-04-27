using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OutroRevealController : MonoBehaviour
{
    [Header("UI Referansları")]
    [Tooltip("Açılmasını istediğin karartı (Image) objelerini buraya sürükle.")]
    [SerializeField] private Image[] darkSpots;

    [Header("Ayarlar")]
    [Tooltip("Her bir karartının kaybolma süresi (saniye)")]
    [SerializeField] private float fadeDurationPerSpot = 1.0f;
    
    [Tooltip("Tüm karartılar açıldıktan sonra oyundan çıkmadan önce beklenecek süre (saniye)")]
    [SerializeField] private float waitTimeBeforeQuit = 3.0f;

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
                // Bir önceki karartı tamamen bitene kadar bekle
                yield return StartCoroutine(FadeOutImage(spot, fadeDurationPerSpot));
            }
        }

        // Tüm karartılar bittikten sonra belirlediğin süre kadar (3 saniye) bekle
        yield return new WaitForSeconds(waitTimeBeforeQuit);

        // Bekleme süresi bitince oyunu kapat
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