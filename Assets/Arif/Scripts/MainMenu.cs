using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Menü Panelleri")]
    public GameObject anaMenuPaneli; // Play, Options, Quit butonlarını barındıran ana panel
    public GameObject optionsPaneli; // İçeriğin boş geldiğini söylediğin panel

    void Start()
    {
        // Sahne her yüklendiğinde kesinlikle Ana Menü görünür, Options gizli olsun
        if (anaMenuPaneli != null) anaMenuPaneli.SetActive(true);
        if (optionsPaneli != null) optionsPaneli.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    // Options butonuna tıkladığında bu metodu çağır
    public void OpenOptions()
    {
        anaMenuPaneli.SetActive(false);
        optionsPaneli.SetActive(true);
    }

    // Options içindeki 'Geri/Kapat' butonuna tıkladığında bu metodu çağır
    public void CloseOptions()
    {
        optionsPaneli.SetActive(false);
        anaMenuPaneli.SetActive(true);
    }
}