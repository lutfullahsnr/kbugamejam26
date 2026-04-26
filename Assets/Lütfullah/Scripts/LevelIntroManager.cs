using UnityEngine;
using Cinemachine;
using DG.Tweening;
using System.Collections;

public class LevelIntroManager : MonoBehaviour
{
    [Header("Kamera ve Hedefler")]
    public CinemachineVirtualCamera vcam;
    [Tooltip("Kameranın zoom yapacağı odak noktası (Boş Obje)")]
    public Transform singleIntroTarget;
    [Tooltip("Oyun başlayınca kameranın döneceği asıl Target Group")]
    public Transform targetGroupObj; 

    [Header("Zoom Ayarları")]
    public float introZoomSize = 2f;    // Ne kadar dibe girecek
    public float normalPlaySize = 8f;   // Oyun sırasındaki genişlik
    
    [Header("Süreler")]
    public float zoomInDuration = 1.5f; // Hedefe zoom yapma süresi
    public float waitTime = 1f;         // Zoom yaptıktan sonra ekranda bekleme süresi
    public float zoomOutDuration = 1.5f;// Gruba geri dönme (açılma) süresi

    void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        // 1. ADIM: HEDEFE KİLİTLEN VE ZOOM İN YAP
        vcam.Follow = singleIntroTarget;
        
        float currentZoom = vcam.m_Lens.OrthographicSize;
        DOTween.To(() => currentZoom, x => 
        { 
            currentZoom = x; 
            vcam.m_Lens.OrthographicSize = currentZoom; 
        }, introZoomSize, zoomInDuration).SetEase(Ease.InOutSine);

        // Zoom süresi + ekranda bekleme süresi kadar dur
        yield return new WaitForSeconds(zoomInDuration + waitTime);

        // 2. ADIM: DÖNÜŞ (HEM POZİSYON HEM ZOOM İÇİN DOTWEEN)
        
        // Işınlanmayı önlemek için kameranın otomatik takibini serbest bırakıyoruz
        vcam.Follow = null; 

        // A) Zoom Out Animasyonu (Genişleme)
        float tempZoom = vcam.m_Lens.OrthographicSize;
        DOTween.To(() => tempZoom, x => 
        { 
            tempZoom = x; 
            vcam.m_Lens.OrthographicSize = tempZoom; 
        }, normalPlaySize, zoomOutDuration).SetEase(Ease.InOutSine);

        // B) Kamera Kayma (Pan) Animasyonu
        // TargetGroup'un o anki merkezini bul ve kamerayı Z eksenini bozmadan oraya kaydır
        Vector3 targetPos = new Vector3(targetGroupObj.position.x, targetGroupObj.position.y, vcam.transform.position.z);
        vcam.transform.DOMove(targetPos, zoomOutDuration).SetEase(Ease.InOutSine);

        // Kayma ve Zoom animasyonlarının bitmesini bekle
        yield return new WaitForSeconds(zoomOutDuration);

        // 3. ADIM: OYUN BAŞLASIN!
        // Kamera zaten merkeze geldi, takibi tekrar açıyoruz ve oyuncu hiçbir zıplama hissetmiyor
        vcam.Follow = targetGroupObj;

        Debug.Log("Sinematik Bitti, Oyuncular Serbest!");
    }
}