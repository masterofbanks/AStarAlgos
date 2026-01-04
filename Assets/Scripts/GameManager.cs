using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Ghosts")]
    public EnemyBehavior[] Ghosts;


    [Header("Pacman Values")]
    public PacmanBehavior Pacman;
    public bool CharactersAreMoveable;
    public float EatenPauseTime = 0.5f;


    [Header("Power Pellet Stuff")]
    public GameObject PowerPellet;
    public CellStats[] PowerPelletSpawnLocations;

    bool _soundLock = true;

    private void Start()
    {
        InitializeLevel();
        CharactersAreMoveable = true;
        SoundManager.PlaySound(SoundType.MOVE);
    }

    private void FixedUpdate()
    {
        if (TestForAnyNormalGhosts() && !_soundLock)
        {
            _soundLock = true;
            SoundManager.PlaySound(SoundType.MOVE);
        }
    }

    //normal ghosts are ones in scatter or cha
    private bool TestForAnyNormalGhosts()
    {
        for(int i = 0; i < Ghosts.Length; i++)
        {
            if (Ghosts[i].state == EnemyBehavior.State.Frightened || Ghosts[i].state == EnemyBehavior.State.Eaten)
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
            Ghosts[i].ForceGhostIntoFrightenedState();
        }
        SoundManager.PlaySound(SoundType.FRIGHTENED, 1f, 0.1f);
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


}
