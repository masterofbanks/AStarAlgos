using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Entities")]
    public EnemyBehavior[] Ghosts;
    public PacmanBehavior Pacman;
    public bool CharactersAreMoveable;
    public float EatenPauseTime = 0.5f;


    [Header("Power Pellet Stuff")]
    public GameObject PowerPellet;
    public CellStats[] PowerPelletSpawnLocations;

    private void Start()
    {
        InitializeLevel();
        CharactersAreMoveable = true;
    }

    public void MakeEveryGhostScared()
    {
        for(int i = 0; i < Ghosts.Length; i++)
        {
            Ghosts[i].ForceGhostIntoFrightenedState();
        }
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


}
