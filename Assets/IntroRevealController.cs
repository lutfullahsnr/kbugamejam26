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
        LoadNextScene();
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

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}