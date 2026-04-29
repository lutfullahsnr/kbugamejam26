using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject pausePanel;
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale=0;
    }

    public void Home()
    {
        Time.timeScale=1;
        SceneManager.LoadScene("mainMenu");
        
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);   
        Time.timeScale=1; 
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale=1;
    }
    public void OpenSettings()
    {
        pausePanel.SetActive(false);  
        settingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        pausePanel.SetActive(true);   
    }
    
}
