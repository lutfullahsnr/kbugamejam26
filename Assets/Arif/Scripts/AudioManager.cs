using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Ses Kaynağı")]
    public AudioSource sfxSource; 

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
    public void ZiplamaSesiCal()
    {
        sfxSource.PlayOneShot(ziplamaSesi);
    }
    public void ToplamaSesiCal()
    {
        sfxSource.PlayOneShot(toplamaSesi);
    }
}