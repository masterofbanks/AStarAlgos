using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
public class EnemyBehavior : MonoBehaviour
{
    public enum State
    {
        Scatter,
        Chase,
        Frightened,
        Eaten
    }
    [Header("State")]
    public State state;
    public float time;
    public GameManager GameScript;

    [Header("Scatter Parameters")]
    public float TimeInScatterState = 7f;
    public CellStats[] ScatterPosition;
    public int CurrentScatterIndex = 0;

    [Header("Chase Parameters")]
    public float TimeInChaseState = 20f;
    public GhostChaseBehavior ChaseScript;

    [Header("Frightened Parameters")]
    public float TimeInFrightenedState = 6f;
    public float TimeInFlickerMode = 2f;

    [Header("Eaten Parameters")]
    public float EatenSpeed;
    public CellStats HomeCell;
    public GameObject EatenSFX;

    [Header("Walkable Grid")]
    public Griddy GridScript;
    public Transform currentTarget;
    public Tuple<int, int> CurrentMovementDirection;
    public CellStats currentTargetCell;
    public List<Transform> TestPath;

    [Header("Physics values")]
    public float NormalSpeed;

    [Header("Path Construction")]
    public CellStats StartingCell;
    public CellStats EndingCell;
    public CellStats RandomCell;
    public LineRenderer VisualOfPath;
    public PacmanBehavior PlayerTarget;


    Rigidbody2D _rb2D;
    Animator anime;
    bool _isTurningAround = false;
    float _speed;
    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
        ChaseScript = GetComponent<GhostChaseBehavior>();
        _speed = NormalSpeed;

    }


    void Start()
    {
        state = State.Scatter;
        CalculateTarget();
        CurrentMovementDirection = new Tuple<int, int>(0, 0);
        EndingCell = currentTargetCell;
        TestCalculationOfPath();
        currentTarget = TestPath[0];
        UpdateCurrentMovementDirection();
        StartCoroutine(FindRandomRoutine());
        
    }

    IEnumerator FindRandomRoutine()
    {
        yield return new WaitForSeconds(Time.fixedDeltaTime);
        RandomCell = GridScript.FindARandomWalkableGridPoint(StartingCell, ScatterPosition);

    }

    private void FixedUpdate()
    {
        CalculateTarget();
        if(GameScript.CharactersAreMoveable)
            MoveToPoint(currentTarget);
        anime.SetInteger("state", (int)state);


    }

    public void MoveToPoint(Transform target)
    {
        Vector2 directionOfMovement = ((Vector2)target.position - _rb2D.position).normalized;
        _rb2D.MovePosition(_rb2D.position + directionOfMovement * _speed * Time.fixedDeltaTime);
    }


    public void TestCalculationOfPath(bool turnAround = false)
    {
        TestPath = CalculatePath(StartingCell, EndingCell, turnAround);
        UpdateLine();



    }

    public virtual void CalculateTarget()
    {
        if(state == State.Scatter)
        {
            currentTargetCell = ScatterPosition[CurrentScatterIndex];
            time += Time.fixedDeltaTime;
            if(time > TimeInScatterState)
            {
                time = 0f;
                state = State.Chase;
            }
        }

        else if(state == State.Chase)
        {
            //currentTargetCell = PlayerTarget.currentCell;
            currentTargetCell =  ChaseScript.Chase(PlayerTarget);
            time += Time.fixedDeltaTime;
            if(time > TimeInChaseState)
            {
                time = 0f;
                state = State.Scatter;
            }
        }

        else if(state == State.Frightened)
        {
            currentTargetCell = RandomCell;
            time += Time.fixedDeltaTime;
            if(time > TimeInFrightenedState)
            {
                time = 0f;
                state = State.Chase;
                anime.SetBool("flicker", false);
            }

            else if(time > TimeInFrightenedState - TimeInFlickerMode)
            {
                anime.SetBool("flicker", true);
            }
        }

        else if(state == State.Eaten)
        {
            currentTargetCell = HomeCell;
        }

        EndingCell = currentTargetCell;

    }

    private void UpdateLine()
    {
        if(TestPath != null)
        {
            VisualOfPath.positionCount = TestPath.Count + 1;
            for (int i = 0; i < TestPath.Count; i++)
            {
                VisualOfPath.SetPosition(i + 1, TestPath[i].position);
            }
            VisualOfPath.SetPosition(0, StartingCell.gameObject.transform.position);
        }
        
    }
    private List<Transform> CalculatePath(CellStats start, CellStats end, bool turnAround = false)
    {
        List<Transform> answer = new();
        List<CellStats> openSet = new();
        List<CellStats> closedSet = new();
        openSet.Add(start);

        //add cell behind to closed set  
        Tuple<int, int> currentCoordinates = GridScript.GetCoordsOfCell(start);
        Tuple<int, int> behindDirection = new Tuple<int, int>(CurrentMovementDirection.Item2 * -1, CurrentMovementDirection.Item1 * -1);
        Tuple<int, int> behindCoordinates = GridScript.AddTuples(currentCoordinates, behindDirection);
        //Debug.Log(behindCoordinates);
        if(!turnAround)
            closedSet.Add(GridScript.GetCellData(behindCoordinates.Item1, behindCoordinates.Item2));


        CellStats currentNode = null;

        while(openSet.Count > 0)
        {
            openSet.Sort(Comparer<CellStats>.Create((s1, s2) => s1.F.CompareTo(s2.F)));
            currentNode = openSet[0];
            openSet.RemoveAt(0);

            closedSet.Add(currentNode);

            //if the current node is the end goal, add each node to the answer list by iterating through the parents of each node
            if(currentNode == end)
            {
                while(currentNode != start)
                {
                    answer.Add(currentNode.gameObject.transform);
                    currentNode = currentNode.parent;
                }
                answer.Reverse(); 
                return answer;
            }
            if(currentNode == null)
            {
                Debug.Log("Current Node is null");
            }
            if(GridScript == null)
            {
                Debug.Log("GridScript is null");
            }
            
            List<CellStats> neighbors = new();
            if(!turnAround)
                neighbors = GridScript.GetNeighborsOfCell(currentNode);
            else
            {
                neighbors.Add(GridScript.GetCellData(behindCoordinates.Item1, behindCoordinates.Item2));
                turnAround = false;
            }

            foreach (CellStats neighbor in neighbors)
            {
                if (!GridScript.IsWalkable(GridScript.GetCoordsOfCell(neighbor).Item1, GridScript.GetCoordsOfCell(neighbor).Item2) || closedSet.Contains(neighbor))
                {

                    continue;
                }

                float cost = currentNode.G + HeuristicCostEstimate(currentNode, neighbor);
                if (cost < neighbor.G || !openSet.Contains(neighbor))
                {
                    neighbor.G = cost;
                    neighbor.H = HeuristicCostEstimate(neighbor, end);
                    neighbor.parent = currentNode;
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }

            }
            neighbors.Clear();


        }

        return null;

    }

    private float HeuristicCostEstimate(CellStats cell1, CellStats cell2)
    {
        Tuple<int, int> cellOneCoords = GridScript.GetCoordsOfCell(cell1);
        Tuple<int, int> cellTwoCoords = GridScript.GetCoordsOfCell(cell2);

        return Mathf.Sqrt(Mathf.Pow((cellTwoCoords.Item1 - cellOneCoords.Item1), 2) + Mathf.Pow((cellTwoCoords.Item2 - cellOneCoords.Item2), 2));

    }

    private void UpdateCurrentMovementDirection()
    {
        Tuple<int, int> targetCoords = GridScript.GetCoordsOfCell(currentTarget.gameObject.GetComponent<CellStats>());
        Tuple<int, int> currentCoords = GridScript.GetCoordsOfCell(StartingCell);
        int currXVelo = targetCoords.Item2 - currentCoords.Item2;
        int currYVelo = targetCoords.Item1 - currentCoords.Item1;
        CurrentMovementDirection = new Tuple<int, int>(currXVelo, currYVelo);
        //Debug.Log(CurrentMovementDirection);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Grid"))
        {
            
            StartingCell = collision.gameObject.GetComponent<CellStats>();
            if(StartingCell.exitCell != null)
            {
                transform.position = StartingCell.exitCell.transform.position;
            }

            //the ghost has reached its target cell
            if(StartingCell == EndingCell)
            {
                ChooseNewEndingCell();
            }

            if(state == State.Frightened)
            {
                TestPath = null; 
                while(TestPath == null)
                {
                    RandomCell = GridScript.FindARandomWalkableGridPoint(StartingCell, ScatterPosition);
                    EndingCell = RandomCell;
                    TestCalculationOfPath(_isTurningAround);
                }

            }
            else
            {
                TestCalculationOfPath(_isTurningAround);
            }

            if (_isTurningAround)
            {
                _isTurningAround = false;
            }


            //in the edge case where no path can be found to a target, force the ghost into its scatter state and direct the ghost to the next available scatter position
            if (TestPath == null)
            {
                Debug.Log($"{gameObject.name}'s TestPath is null after trying to find a new path");
                state = State.Scatter;
                CurrentScatterIndex++;
                if (CurrentScatterIndex == ScatterPosition.Length)
                {
                    CurrentScatterIndex = 0;
                }
                CalculateTarget();
                TestCalculationOfPath();
            }


            currentTarget = TestPath[0];
            UpdateCurrentMovementDirection();

        }

        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"{gameObject.name} hit {collision.gameObject.name}");
            if (state == State.Frightened)
            {
                ForceGhostIntoEatenState();
            }

            else if ((state == State.Scatter || state == State.Chase) && GameScript.CanDie)
            {
                collision.gameObject.GetComponent<PacmanBehavior>().PerformDeath();
            }
        }


    }

    private void ChooseNewEndingCell()
    {
        Debug.Log("Target Reached!");
        if (state == State.Scatter)
        {
            CurrentScatterIndex++;
            if (CurrentScatterIndex == ScatterPosition.Length)
            {
                CurrentScatterIndex = 0;
            }
        }

        else if (state == State.Chase)
        {
            state = State.Scatter;
        }

        else if (state == State.Frightened)
        {
            RandomCell = GridScript.FindARandomWalkableGridPoint(StartingCell, ScatterPosition);
        }

        else if(state == State.Eaten)
        {
            state = State.Chase;
            _speed = NormalSpeed;
        }
        CalculateTarget();
    }

    public void ForceGhostIntoFrightenedState()
    {
        time = 0;
        state = State.Frightened;
        _isTurningAround = true;
        anime.SetBool("flicker", false);
    }

    private void ForceGhostIntoEatenState()
    {
        state = State.Eaten;
        _speed = EatenSpeed;
        Instantiate(EatenSFX, transform.position, Quaternion.identity);
        StartCoroutine(GameScript.EatenRoutine());
    }

    public void TurnGhostOff()
    {
        SpriteRenderer[] ghostSprites = GetComponentsInChildren<SpriteRenderer>();
        for(int i = 0; i < ghostSprites.Length; i++)
        {
            ghostSprites[i].enabled = false;
        }
    }

    public void ForceGhostIntoScatterState()
    {
        time = 0;
        state = State.Scatter;
        _isTurningAround = true;
        if(StartingCell == ScatterPosition[CurrentScatterIndex])
        {
            CurrentScatterIndex++;
            if(CurrentScatterIndex == ScatterPosition.Length)
            {
                CurrentScatterIndex = 0;
            }
        }
    }

   
}
