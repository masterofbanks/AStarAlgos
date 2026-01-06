using UnityEngine;

public class BlinkyChase : GhostChaseBehavior
{
    public override CellStats Chase(PacmanBehavior pacman)
    {
        return pacman.currentCell;
    }
}
