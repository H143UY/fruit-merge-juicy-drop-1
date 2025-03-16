using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreenController : MonoBehaviour
{
    public GameObject LoseGameImg;
    private void Awake()
    {
        this.RegisterListener(EventID.GameOver, (sender, param) =>
        {
            LoseGameImg.SetActive(true);
        });
    }
    void Start()
    {
        LoseGameImg.SetActive(false);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.DeleteAll();
    }
}
