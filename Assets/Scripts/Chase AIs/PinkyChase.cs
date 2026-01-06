using System;
using System.Diagnostics.Contracts;
using UnityEngine;

public class PinkyChase : GhostChaseBehavior
{
    public int NumberOfCellsInFront = 2;
    public Griddy GridReference;
    public EnemyBehavior EnemyScript;

    private void Awake()
    {
        EnemyScript = GetComponent<EnemyBehavior>();
    }
    public override CellStats Chase(PacmanBehavior pacman)
    {
        Tuple<int, int> coordsOfNewTarget = GridReference.AddTuples(GridReference.GetCoordsOfCell(pacman.currentCell), new Tuple<int, int>((int)pacman.CurrentDirection.y * NumberOfCellsInFront, (int)pacman.CurrentDirection.x * NumberOfCellsInFront));

        CellStats newTargetCell = GridReference.GetCellData(coordsOfNewTarget.Item1, coordsOfNewTarget.Item2);
        
        if(newTargetCell == null)
        {
            //Debug.Log("Pinky chase ai could not find a new target cell");
            return pacman.currentCell;
        }

        else if (!newTargetCell.Walkable)
        {
            return GridReference.FindNearestWalkableCell(newTargetCell, EnemyScript.ScatterPosition[EnemyScript.CurrentScatterIndex]);
        }
        return newTargetCell;
    }
}
