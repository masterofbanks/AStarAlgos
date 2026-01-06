using UnityEngine;

public abstract class GhostChaseBehavior : MonoBehaviour
{
    public abstract CellStats Chase(PacmanBehavior pacman);
    
}
