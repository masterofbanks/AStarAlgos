using System;
using UnityEngine;

public class InkyChase : GhostChaseBehavior
{
    public int NumberOfCellsInFront = 2;
    public Griddy GridReference;
    public EnemyBehavior Blinky;
    
    public override CellStats Chase(PacmanBehavior pacman)
    {
        Tuple<int, int> coordsOfCellInFront = GridReference.AddTuples(GridReference.GetCoordsOfCell(pacman.currentCell), new Tuple<int, int>((int)pacman.CurrentDirection.y * NumberOfCellsInFront, (int)pacman.CurrentDirection.x * NumberOfCellsInFront));

        CellStats cellInFront = GridReference.GetCellData(coordsOfCellInFront.Item1, coordsOfCellInFront.Item2);

        if(cellInFront == null)
        {
            cellInFront = pacman.currentCell;
        }

        Tuple<int, int> vectorToBlinky = GridReference.SubTuples(GridReference.GetCoordsOfCell(Blinky.StartingCell), GridReference.GetCoordsOfCell(cellInFront));

        Tuple<int, int> reflectedBlinkyVector = new Tuple<int, int>(-1 * vectorToBlinky.Item1, -1 * vectorToBlinky.Item2);

        Tuple<int, int> newTargetCoords = GridReference.AddTuples(reflectedBlinkyVector, GridReference.GetCoordsOfCell(cellInFront));

        CellStats newTargetCell = GridReference.GetCellData(newTargetCoords.Item1, newTargetCoords.Item2);

        if(newTargetCell == null)
        {
            return pacman.currentCell;
        }

        else if (!newTargetCell.Walkable)
        {
            return GridReference.FindNearestWalkableCell(newTargetCell, Blinky.ScatterPosition[Blinky.CurrentScatterIndex]);
        }

        return newTargetCell;
    }

}
