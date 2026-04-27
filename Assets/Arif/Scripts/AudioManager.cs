using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Bu scripte oyunun her yerinden kolayca ulaşabilmek için Singleton (Tekil) yapısı kuruyoruz
    public static AudioManager instance;

    [Header("Ses Kaynağı")]
    public AudioSource sfxSource; // Sesleri çalacak hoparlörümüz

    [Header("Ses Dosyaları")]
    public AudioClip ziplamaSesi;
    public AudioClip toplamaSesi;

    void Awake()
    {
        // Singleton Kurulumu
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Zıplama sesini çalmak için çağıracağımız fonksiyon
    public void ZiplamaSesiCal()
    {
        // PlayOneShot, aynı anda birden fazla sesin üst üste çalmasına izin verir
        sfxSource.PlayOneShot(ziplamaSesi);
    }

    // Ölme sesini çalmak için çağıracağımız fonksiyon
    public void ToplamaSesiCal()
    {
        sfxSource.PlayOneShot(toplamaSesi);
    }
}