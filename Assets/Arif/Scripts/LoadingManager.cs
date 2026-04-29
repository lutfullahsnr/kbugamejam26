using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [Header("Ayarlar")]
    public string nextSceneName = "mainMenu"; 
    public float waitTime = 2f;           

    void Start()
    {
        StartCoroutine(WaitAndLoad());
    }

    IEnumerator WaitAndLoad()
    {
        yield return new WaitForSeconds(waitTime);

        SceneManager.LoadScene(nextSceneName);
    }
}