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
        if (creditsPanel != null) 
        {
            creditsPanel.SetActive(false);
        }
        StartCoroutine(RevealSequence());
    }

    private IEnumerator RevealSequence()
    {
        foreach (Image spot in darkSpots)
        {
            if (spot != null)
            {
                yield return StartCoroutine(FadeOutImage(spot, fadeDurationPerSpot));
            }
        }

        if (creditsPanel != null)
        {
            creditsPanel.SetActive(true);
            yield return StartCoroutine(ScrollCredits());
        }
        yield return new WaitForSeconds(waitTimeBeforeQuit);

        QuitGame();
    }

    private IEnumerator FadeOutImage(Image img, float duration)
    {
        Color startColor = img.color;
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

        while (elapsedTime < creditsDuration)
        {
            elapsedTime += Time.deltaTime;
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
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Oyun Build alınıp EXE/APK vs. yapıldığında çalışacak gerçek kapatma komutu
        Application.Quit();
#endif
    }
}