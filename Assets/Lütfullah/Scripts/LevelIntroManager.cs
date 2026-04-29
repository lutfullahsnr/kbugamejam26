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
    public float introZoomSize = 2f;   
    public float normalPlaySize = 8f;   
    
    [Header("Süreler")]
    public float zoomInDuration = 1.5f; 
    public float waitTime = 1f;        
    public float zoomOutDuration = 1.5f;

    void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        vcam.Follow = singleIntroTarget;
        
        float currentZoom = vcam.m_Lens.OrthographicSize;
        DOTween.To(() => currentZoom, x => 
        { 
            currentZoom = x; 
            vcam.m_Lens.OrthographicSize = currentZoom; 
        }, introZoomSize, zoomInDuration).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(zoomInDuration + waitTime);

        vcam.Follow = null; 

        float tempZoom = vcam.m_Lens.OrthographicSize;
        DOTween.To(() => tempZoom, x => 
        { 
            tempZoom = x; 
            vcam.m_Lens.OrthographicSize = tempZoom; 
        }, normalPlaySize, zoomOutDuration).SetEase(Ease.InOutSine);

        Vector3 targetPos = new Vector3(targetGroupObj.position.x, targetGroupObj.position.y, vcam.transform.position.z);
        vcam.transform.DOMove(targetPos, zoomOutDuration).SetEase(Ease.InOutSine);

        yield return new WaitForSeconds(zoomOutDuration);

        vcam.Follow = targetGroupObj;

        Debug.Log("Sinematik Bitti, Oyuncular Serbest!");
    }
}