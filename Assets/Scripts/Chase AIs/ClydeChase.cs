using System;
using UnityEngine;

public class ClydeChase : GhostChaseBehavior
{
    public float ScatterDistance = 5f;
    public Griddy GridReference;
    public EnemyBehavior EnemyScript;

    private void Awake()
    {
        EnemyScript = GetComponent<EnemyBehavior>();
    }

    public override CellStats Chase(PacmanBehavior pacman)
    {
        Tuple<int, int> pacmanCoords = GridReference.GetCoordsOfCell(pacman.currentCell);
        Tuple<int, int> currentCoords = GridReference.GetCoordsOfCell(EnemyScript.StartingCell);

        float distanceToPacman = Mathf.Sqrt(Mathf.Pow(Mathf.Abs(pacmanCoords.Item1 - currentCoords.Item1), 2) + Mathf.Pow(Mathf.Abs(pacmanCoords.Item2 - currentCoords.Item2), 2));
        if(distanceToPacman < ScatterDistance)
        {
            EnemyScript.ForceGhostIntoScatterState();
            return EnemyScript.ScatterPosition[EnemyScript.CurrentScatterIndex];
        }

        return pacman.currentCell;
    }
}
