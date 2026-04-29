using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Menü Panelleri")]
    public GameObject anaMenuPaneli;
    public GameObject optionsPaneli; 

    void Start()
    {
        if (anaMenuPaneli != null) anaMenuPaneli.SetActive(true);
        if (optionsPaneli != null) optionsPaneli.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
    public void OpenOptions()
    {
        anaMenuPaneli.SetActive(false);
        optionsPaneli.SetActive(true);
    }
    public void CloseOptions()
    {
        optionsPaneli.SetActive(false);
        anaMenuPaneli.SetActive(true);
    }
}