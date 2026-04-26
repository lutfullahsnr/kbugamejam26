using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Diğer scriptlerden kolayca ulaşmak için

    [Header("UI Panelleri")]
    public GameObject winPanel;
    public GameObject losePanel;

    void Awake()
    {
        // Singleton yapısı: Sahnede sadece bir tane GameManager olmasını sağlar
        if (instance == null) 
        {
            instance = this;
        }
    }

    // --- PANEL AÇMA FONKSİYONLARI ---

    public void ShowWinPanel()
    {
        winPanel.SetActive(true); // Kazanma panelini aç
        Time.timeScale = 0f;      // Arka planda oyunu durdur (karakterler hareket edemesin)
    }

    public void ShowLosePanel()
    {
        losePanel.SetActive(true); // Kaybetme panelini aç
        Time.timeScale = 0f;       // Arka planda oyunu durdur
    }

    // --- BUTON FONKSİYONLARI ---

    public void NextLevel()
    {
        Time.timeScale = 1f; // Zamanı normale döndür (Çok Önemli!)
        
        if(SceneManager.GetActiveScene().name == "Level 3")
            SceneManager.LoadScene("LastScene"); // Son bölümse bitiş sahnesine git
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // Sonraki bölüme geç
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Zamanı normale döndür
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Aynı sahneyi tekrar yükle
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f; // Zamanı normale döndür
        SceneManager.LoadScene("MainMenu"); // Kendi Ana menü sahnenizin adını buraya yazın (Örn: "Menu")
    }
}