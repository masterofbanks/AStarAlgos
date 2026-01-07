using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Control")]
    public LevelLoader LoadScript;
    public int NumberOfDots = 9999;
    public int NumberOfDotsCollected = 0;
    public int NumberOfLives = 3;
    public bool CanDie = true;
    public bool gameHasEnded;
    public float WinAdmirationTime = 2.5f;
    public GameObject ResetScreen;

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
        InitializeLevel();
        CharactersAreMoveable = true;
        gameHasEnded = false;
        SoundManager.PlaySound(SoundType.MOVE, 0.04f);
        StartCoroutine(SpawnInDots());
    }



    IEnumerator SpawnInDots()
    {
        yield return new WaitForSeconds(Time.fixedDeltaTime);
        List<Transform> SpawnDotLocations = CellGrid.FindDotSpawnLocations();
        for (int i = 0; i < SpawnDotLocations.Count; i++)
        {
            Instantiate(NormalPellet, SpawnDotLocations[i].position, Quaternion.identity, PelletParent);
        }
        NumberOfDots = SpawnDotLocations.Count;
    }

    private void FixedUpdate()
    {
        
        if (TestForAnyNormalGhosts() && !_soundLock)
        {
            _soundLock = true;
            SoundManager.PlaySound(SoundType.MOVE, 0.04f);
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

    private bool TestForEatenGhosts()
    {
        for(int i = 0; i < Ghosts.Length; i++)
        {
            if (Ghosts[i].state == EnemyBehavior.State.Eaten)
            {
                return true;
            }
        }

        return false;
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
        StartCoroutine(EndWinStateRoutine());
    }

    IEnumerator EndWinStateRoutine()
    {
        LoadScript.IncramentCurrentLevel();
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
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
