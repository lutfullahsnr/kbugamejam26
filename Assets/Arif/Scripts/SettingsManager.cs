using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // Sürgüleri koddan kontrol etmek için

public class SettingsManager : MonoBehaviour
{
    public AudioMixer anaMixer;
    
    [Header("Sürgüler (UI)")]
    public Slider muzikSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Oyun/Sahne başladığında kaydedilmiş ayarları yükle
        // Eğer daha önce hiç ayar yapılmamışsa varsayılan olarak 1 (maksimum) yap.
        float kayitliMuzik = PlayerPrefs.GetFloat("MuzikAyar", 1f);
        float kayitliSFX = PlayerPrefs.GetFloat("SFXAyar", 1f);

        // UI Sürgülerini (Slider) kaydedilen yerlerine çek
        if(muzikSlider != null) muzikSlider.value = kayitliMuzik;
        if(sfxSlider != null) sfxSlider.value = kayitliSFX;

        // Ayarları Mixer'e uygula
        SetMusicVolume(kayitliMuzik);
        SetSFXVolume(kayitliSFX);
    }

    public void SetMusicVolume(float sliderValue)
    {
        anaMixer.SetFloat("MuzikVol", Mathf.Log10(sliderValue) * 20);
        // Yeni ayarı hafızaya kaydet
        PlayerPrefs.SetFloat("MuzikAyar", sliderValue); 
    }

    public void SetSFXVolume(float sliderValue)
    {
        anaMixer.SetFloat("SesVol", Mathf.Log10(sliderValue) * 20);
        // Yeni ayarı hafızaya kaydet
        PlayerPrefs.SetFloat("SFXAyar", sliderValue);
    }
}