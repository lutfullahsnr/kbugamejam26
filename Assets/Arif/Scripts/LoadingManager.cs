using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [Header("Ayarlar")]
    public string nextSceneName = "mainMenu"; // Geçilecek sahnenin tam adı
    public float waitTime = 2f;               // Ekranda bekleme süresi (saniye)

    void Start()
    {
        // Sahne açılır açılmaz geri sayımı başlat
        StartCoroutine(WaitAndLoad());
    }

    IEnumerator WaitAndLoad()
    {
        // Belirlediğimiz süre kadar (3 saniye) hiçbir şey yapmadan bekle
        yield return new WaitForSeconds(waitTime);

        // Süre dolduğunda hedef sahneyi yükle
        SceneManager.LoadScene(nextSceneName);
    }
}