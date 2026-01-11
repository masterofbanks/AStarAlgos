using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameManager GameScript;

    [Header("Life UI")]
    public Transform LifeUIImagesParent;
    public GameObject LifeImage;

    [Header("Score UI")]
    public TextMeshProUGUI CurrentScoreText;
    public string DefaultCurrentScoreMessage = "Current Score: ";
    public GameObject EatScoreUINotif;
    public Transform OffScreenLocation;

    [Header("High Score UI")]
    public TextMeshProUGUI HighScoreText;
    public string DefaultHighScoreMessage = "High Score: ";

    private void Start()
    {
        foreach(Transform child in LifeUIImagesParent)
        {
            Destroy(child.gameObject);
        }

        for(int i = 0; i < GameScript.NumberOfLives ; i++)
        {
            Instantiate(LifeImage, LifeUIImagesParent);
        }

        HighScoreText.text = $"{DefaultHighScoreMessage}{PlayerPrefs.GetInt($"{SceneManager.GetActiveScene().name} High Score")}";
    }

    public void RemoveLife()
    {
        int indexOfLifeToRemove = LifeUIImagesParent.childCount - 1;
        if(indexOfLifeToRemove < 0)
        {
            return;
        }

        Destroy(LifeUIImagesParent.GetChild(indexOfLifeToRemove).gameObject);
    }

    public void UpdateScoreUI(int score)
    {
        CurrentScoreText.text = $"{DefaultCurrentScoreMessage}{score.ToString()}";
    }

    public void MakeScoreNotif(int score, Vector3 position)
    {
        GameObject notif = Instantiate(EatScoreUINotif, OffScreenLocation.position, Quaternion.identity, OffScreenLocation);
        notif.GetComponent<TextMeshPro>().text = score.ToString();
        notif.transform.position = position;
    }

    public void UpdateHighScore(int newHighScore)
    {
        if (newHighScore > PlayerPrefs.GetInt($"{SceneManager.GetActiveScene().name} High Score"))
        {
            PlayerPrefs.SetInt($"{SceneManager.GetActiveScene().name} High Score", newHighScore);
            HighScoreText.text = $"{DefaultHighScoreMessage}{PlayerPrefs.GetInt($"{SceneManager.GetActiveScene().name} High Score")}";
            PlayerPrefs.SetInt("New High Score", 1);
        }
        
    }

    public void AddLifeUI()
    {
        Instantiate(LifeImage, LifeUIImagesParent);
    }

}
