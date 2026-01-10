using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject NewHighScoreNoti;
    public GameObject NormalScoreNoti;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DisplayScore(PlayerPrefs.GetInt("New High Score") == 1);
        PlayerPrefs.SetInt("New High Score", 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void NewGame()
    {
        SceneManager.LoadScene("RetroLevel");
    }

    private void DisplayScore(bool newHighScore)
    {
        NewHighScoreNoti.SetActive(newHighScore);
        NormalScoreNoti.SetActive(!newHighScore);
        if (newHighScore)
        {
            NewHighScoreNoti.GetComponent<TextMeshProUGUI>().text = $"New High Score!\r\n{PlayerPrefs.GetInt("RetroLevel High Score")}";
        }

        else
        {
            NormalScoreNoti.GetComponent<TextMeshProUGUI>().text = $"Score: {PlayerPrefs.GetInt("Current Score")}\r\nHigh Score: {PlayerPrefs.GetInt("RetroLevel High Score")}";
        }
        PlayerPrefs.SetInt("Current Score", 0);

    }
}
