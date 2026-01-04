using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Entities")]
    public EnemyBehavior[] Ghosts;
    public PacmanBehavior Pacman;
    public bool CharactersAreMoveable;

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


}
