using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Walkable Grid")]
    public GenerateGrid Grid;
    public Transform[] TestPath;
    public Transform currentTarget;
    public int CurrentTargetIndex;

    [Header("Physics values")]
    public float Speed;
    public float epsilonDistance;

    Rigidbody2D _rb2D;
    bool reachedObjective = false;

    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentTargetIndex = 0;
        currentTarget = TestPath[CurrentTargetIndex];
    }

    private void FixedUpdate()
    {
        if (!reachedObjective)
        {
            MoveToPoint(currentTarget);
            if (DetermineReachedPosition(currentTarget))
            {
                CurrentTargetIndex++;
                if (CurrentTargetIndex == TestPath.Length)
                {
                    reachedObjective = true;
                }
                else
                {
                    currentTarget = TestPath[CurrentTargetIndex];
                }
            }
        }
        
    }

    public void MoveToPoint(Transform target)
    {
        Vector2 directionOfMovement = ((Vector2)target.position - _rb2D.position).normalized;
        _rb2D.MovePosition(_rb2D.position + directionOfMovement * Speed * Time.fixedDeltaTime);
    }

    private bool DetermineReachedPosition(Transform target)
    {
        return Vector2.Distance((Vector2)target.position, _rb2D.position) < epsilonDistance;
    }
}
