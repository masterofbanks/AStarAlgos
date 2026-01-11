using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Control")]
    public int NumberOfDots = 9999;
    public int NumberOfDotsCollected = 0;
    public int NumberOfLives = 3;
    public bool CanDie = true;
    public bool gameHasEnded;
    public float WinAdmirationTime = 2.5f;
    public GameObject ResetScreen;
    public UIManager UIManagementScript;
    public Transform CanvasParent;
    public GameObject Fader;
    public GameObject[] Lines;
    bool linesAreOn = false;
    LevelLoader LoadScript;


    [Header("Score")]
    public int Score;
    public int LifeUpScore = 10000;
    public int NumberOfEatenGhosts;
    public GameObject LifeUpSFX;
    private int PtsToGetALife = 0;

    [Header("Ghosts")]
    public EnemyBehavior[] Ghosts;


    [Header("Pacman Values")]
    public PacmanBehavior Pacman;
    public bool CharactersAreMoveable;
    public float EatenPauseTime = 0.5f;


    [Header("Power Pellet Stuff")]
    public GameObject PowerPellet;
    public CellStats[] PowerPelletSpawnLocations;
    public GameObject NormalPellet;
    public Transform PelletParent;
    public Griddy CellGrid;

    [Header("Fruit Values")]
    public CellStats FruitCell;
    public GameObject FruitToSpawn;
    public float TimeToSpawnFruit = 45f;

    bool _soundLock = true;
    float fruitTimer = 0f;
    bool spawnedFruit = false;
    private void Start()
    {
        LoadScript = GameObject.FindWithTag("GameController").GetComponent<LevelLoader>();
        InitializeLevel();
        PtsToGetALife = 0;
        gameHasEnded = false;
        StartCoroutine(SpawnInDots());
    }



    IEnumerator SpawnInDots()
    {
        CharactersAreMoveable = false;
        Pacman.SetPacmanAnimatorSpeed(0);
        yield return new WaitForSeconds(Time.fixedDeltaTime);
        List<Transform> SpawnDotLocations = CellGrid.FindDotSpawnLocations();
        for (int i = 0; i < SpawnDotLocations.Count; i++)
        {
            Instantiate(NormalPellet, SpawnDotLocations[i].position, Quaternion.identity, PelletParent);
        }
        NumberOfDots = SpawnDotLocations.Count;
        SoundManager.PlaySound(SoundType.START, 0.25f, 0, false);
        yield return new WaitForSeconds(4.3f);
        SoundManager.PlaySound(SoundType.MOVE, 0.2f);
        CharactersAreMoveable = true;
        Pacman.SetPacmanAnimatorSpeed(1);


    }

    private void FixedUpdate()
    {
        
        if (TestForAnyNormalGhosts() && !_soundLock && Pacman.IsAlive)
        {
            _soundLock = true;
            SoundManager.PlaySound(SoundType.MOVE, 0.04f);
            NumberOfEatenGhosts = 0;
        }

        if(NumberOfDots == 0 && !gameHasEnded)
        {
            PerformWinState();
        }

        UpdateFruitTimer();
    }

    private void UpdateFruitTimer()
    {
        if (!spawnedFruit)
        {
            fruitTimer += Time.fixedDeltaTime;
            if (fruitTimer > TimeToSpawnFruit)
            {
                Instantiate(FruitToSpawn, FruitCell.transform.position, Quaternion.identity);
                fruitTimer = 0f;
                spawnedFruit = true;
            }
        }
        
    }

    //normal ghosts are ones in scatter or cha
    private bool TestForAnyNormalGhosts()
    {
        for(int i = 0; i < Ghosts.Length; i++)
        {
            if (Ghosts[i].state == EnemyBehavior.State.Frightened)
            {
                return false;
            }
        }

        return true;
    }


    public void InstantiateEatScoreUI(Vector3 position)
    {
        NumberOfEatenGhosts++;
        int eatScore = 0;
        switch (NumberOfEatenGhosts)
        {
            case 1:
                eatScore = 200;
                break;
            case 2:
                eatScore = 400;
                break;

            case 3:
                eatScore = 800;
                break;

            case 4:
                eatScore = 1600;
                break;

            default:
                eatScore = 0;
                break;

        }

        AddScore(eatScore);
        UIManagementScript.MakeScoreNotif(eatScore, position);
        Debug.Log(eatScore);
    }

    public void MakeFruitScoreUI(int amount, Vector3 position)
    {
        UIManagementScript.MakeScoreNotif(amount, position);
    }
    public void MakeEveryGhostScared()
    {
        for(int i = 0; i < Ghosts.Length; i++)
        {
            if (Ghosts[i].state != EnemyBehavior.State.Eaten && Ghosts[i].state != EnemyBehavior.State.Idle)
                Ghosts[i].ForceGhostIntoFrightenedState();
        }
        SoundManager.PlaySound(SoundType.FRIGHTENED, 0.05f, 0.3f);
        _soundLock = false;
    }

    public void InitializeLevel()
    {
        Score = PlayerPrefs.GetInt("Current Score");
        UIManagementScript.UpdateScoreUI(Score);

        for(int i = 0; i < PowerPelletSpawnLocations.Length; i++)
        {
            Instantiate(PowerPellet, PowerPelletSpawnLocations[i].transform.position, Quaternion.identity);
        }

        FruitToSpawn = LoadScript.GetCurrentLevelStats().Fruit;

        for(int i = 0; i < Ghosts.Length; i++)
        {
            float newEnemySpeed = LoadScript.GetCurrentLevelStats().EnemySpeed;
            Ghosts[i].NormalSpeed = newEnemySpeed;
            Ghosts[i].SetSpeed(newEnemySpeed);

            Ghosts[i].TimeInFrightenedState = LoadScript.GetCurrentLevelStats().TimeInFrightenedMode;
        }
    }

    public IEnumerator EatenRoutine()
    {
        CharactersAreMoveable = false;
        Pacman.SetPacmanAnimatorSpeed(0f);
        yield return new WaitForSeconds(EatenPauseTime);
        CharactersAreMoveable = true;
        Pacman.SetPacmanAnimatorSpeed(1f);
    }

    private void PerformWinState()
    {
        gameHasEnded = true;
        CharactersAreMoveable = false;
        LoadScript.IncramentCurrentLevel();
        PlayerPrefs.SetInt("Current Score", Score);
        StartCoroutine(EndWinStateRoutine());
    }

    IEnumerator EndWinStateRoutine()
    {
        SoundManager.PauseSound();
        yield return new WaitForSeconds(1.0f);
        for(int i = 0; i < Ghosts.Length; i++)
        {
            Ghosts[i].TurnGhostOff();
        }

        yield return new WaitForSeconds(1.0f);
        ResetScreen.SetActive(true);

        yield return new WaitForSeconds(WinAdmirationTime - 2.0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PerformLoseState()
    {
        NumberOfLives--;
        PtsToGetALife = 0;
        if(NumberOfLives == 0)
        {
            StartCoroutine(RestartLevelRoutine());
            return;
        }

        else
        {
            StartCoroutine(ResetLevelRoutine());
        }
    }

    IEnumerator ResetLevelRoutine()
    {
        UIManagementScript.RemoveLife();
        NumberOfEatenGhosts = 0;
        fruitTimer = 0f;
        spawnedFruit = false;
        ResetScreen.SetActive(true);
        Pacman.ResetPacman();
        for (int i = 0; i < Ghosts.Length; i++)
        {
            Ghosts[i].ResetGhost();

        }
        yield return new WaitForSeconds(1.0f);
        CharactersAreMoveable = true;
        ResetScreen.SetActive(false);
        SoundManager.PlaySound(SoundType.MOVE, 0.04f);
    }

    IEnumerator RestartLevelRoutine()
    {
        UIManagementScript.UpdateHighScore(Score);
        PlayerPrefs.SetInt("Current Score", Score);
        yield return new WaitForSeconds(1.0f);
        Instantiate(Fader, CanvasParent);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("GameOver");
    }

    public void AddScore(int amount)
    {
        Score += amount;
        PtsToGetALife += amount;
        if(PtsToGetALife > LifeUpScore)
        {
            PtsToGetALife = 0;
            NumberOfLives++;
            UIManagementScript.AddLifeUI();
            Instantiate(LifeUpSFX);
        }
        UIManagementScript.UpdateScoreUI(Score);
    }

    public void FlickerLines()
    {
        linesAreOn = !linesAreOn;
        for(int i = 0; i < Lines.Length; i++)
        {
            Lines[i].SetActive(linesAreOn);
        }
    }

}
