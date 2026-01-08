using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameManager GameScript;

    [Header("Life UI")]
    public Transform LifeUIImagesParent;
    public GameObject LifeImage;

    [Header("Score UI")]
    public TextMeshProUGUI CurrentScoreText;
    public string DefaultStringMessage = "Current Score: ";
    public GameObject EatScoreUINotif;
    public Transform OffScreenLocation;

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
        CurrentScoreText.text = $"{DefaultStringMessage}{score.ToString()}";
    }

    public void MakeScoreNotif(int score, Vector3 position)
    {
        GameObject notif = Instantiate(EatScoreUINotif, OffScreenLocation.position, Quaternion.identity, OffScreenLocation);
        notif.GetComponent<TextMeshPro>().text = score.ToString();
        notif.transform.position = position;
    }
}
