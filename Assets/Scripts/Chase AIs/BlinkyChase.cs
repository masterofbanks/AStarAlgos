using UnityEngine;

public class BlinkyChase : GhostChaseBehavior
{
    public GameManager GameScript;
    public EnemyBehavior EnemyScript;
    public PacmanBehavior Pacman;
    public int DotsLeftToActivateElroy = 10;
    public float HowMuchFasterThanPacman = 2f;
    public Sprite AngryFace;
    public SpriteRenderer FaceRenderer;

    private void Awake()
    {
        EnemyScript = GetComponent<EnemyBehavior>();
    }
    private void FixedUpdate()
    {
        if(GameScript.NumberOfDots <= DotsLeftToActivateElroy)
        {
            EnemyScript.SetSpeed(Pacman.MovementSpeed + HowMuchFasterThanPacman);
            FaceRenderer.sprite = AngryFace;
        }
    }
    public override CellStats Chase(PacmanBehavior pacman)
    {
        return pacman.currentCell;
    }
}
