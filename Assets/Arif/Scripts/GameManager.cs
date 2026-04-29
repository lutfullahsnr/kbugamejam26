using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; 

    [Header("UI Panelleri")]
    public GameObject winPanel;
    public GameObject losePanel;

    void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
    }
    public void ShowWinPanel()
    {
        winPanel.SetActive(true); 
        Time.timeScale = 0f;     
    }
    public void ShowLosePanel()
    {
        losePanel.SetActive(true); 
        Time.timeScale = 0f;       
    }
    public void NextLevel()
    {
        Time.timeScale = 1f; 
        
        if(SceneManager.GetActiveScene().name == "Level 3")
            SceneManager.LoadScene("LastScene"); 
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 
    }
    public void RestartLevel()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }
    public void GoToMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu"); 
    }
}