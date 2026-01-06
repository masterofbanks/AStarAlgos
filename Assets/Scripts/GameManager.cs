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
    public bool CanDie = true;
    public bool gameHasEnded;
    public float WinAdmirationTime = 2.5f;

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

    bool _soundLock = true;

    private void Start()
    {
        InitializeLevel();
        CharactersAreMoveable = true;
        gameHasEnded = false;
        SoundManager.PlaySound(SoundType.MOVE, 0.5f);
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
            SoundManager.PlaySound(SoundType.MOVE, 0.5f);
        }

        if(NumberOfDots == 0 && !gameHasEnded)
        {
            PerformWinState();
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
            if (Ghosts[i].state != EnemyBehavior.State.Eaten)
                Ghosts[i].ForceGhostIntoFrightenedState();
        }
        SoundManager.PlaySound(SoundType.FRIGHTENED, 0.667f, 0.1f);
        _soundLock = false;
    }

    public void InitializeLevel()
    {
        for(int i = 0; i < PowerPelletSpawnLocations.Length; i++)
        {
            Instantiate(PowerPellet, PowerPelletSpawnLocations[i].transform.position, Quaternion.identity);
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
        yield return new WaitForSeconds(1.0f);
        for(int i = 0; i < Ghosts.Length; i++)
        {
            Ghosts[i].TurnGhostOff();
        }

        yield return new WaitForSeconds(WinAdmirationTime - 1.0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


}
