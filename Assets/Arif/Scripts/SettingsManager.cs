using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; 

public class SettingsManager : MonoBehaviour
{
    public AudioMixer anaMixer;
    
    [Header("Sürgüler (UI)")]
    public Slider muzikSlider;
    public Slider sfxSlider;

    void Start()
    {
        float kayitliMuzik = PlayerPrefs.GetFloat("MuzikAyar", 1f);
        float kayitliSFX = PlayerPrefs.GetFloat("SFXAyar", 1f);

        if(muzikSlider != null) muzikSlider.value = kayitliMuzik;
        if(sfxSlider != null) sfxSlider.value = kayitliSFX;

        SetMusicVolume(kayitliMuzik);
        SetSFXVolume(kayitliSFX);
    }

    public void SetMusicVolume(float sliderValue)
    {
        anaMixer.SetFloat("MuzikVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MuzikAyar", sliderValue); 
    }

    public void SetSFXVolume(float sliderValue)
    {
        anaMixer.SetFloat("SesVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXAyar", sliderValue);
    }
}