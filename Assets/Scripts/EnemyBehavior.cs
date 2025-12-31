using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
public class EnemyBehavior : MonoBehaviour
{
    public enum State
    {
        Scatter,
        Chase
    }
    [Header("State")]
    public State state;
    public float time;

    [Header("Scatter Parameters")]
    public float TimeInScatterState = 7f;
    public CellStats[] ScatterPosition;
    public int CurrentScatterIndex = 0;

    [Header("Chase Parameters")]
    public float TimeInChaseState = 20f;

    [Header("Walkable Grid")]
    public Griddy GridScript;
    public Transform currentTarget;
    public Tuple<int, int> CurrentMovementDirection;
    public CellStats currentTargetCell;
    public List<Transform> TestPath;

    [Header("Physics values")]
    public float Speed;

    [Header("Path Construction")]
    public CellStats StartingCell;
    public CellStats EndingCell;
    public LineRenderer VisualOfPath;
    public PacmanBehavior PlayerTarget;


    Rigidbody2D _rb2D;
    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();

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
        
    }

    private void FixedUpdate()
    {
        CalculateTarget();
        MoveToPoint(currentTarget);



    }

    public void MoveToPoint(Transform target)
    {
        Vector2 directionOfMovement = ((Vector2)target.position - _rb2D.position).normalized;
        _rb2D.MovePosition(_rb2D.position + directionOfMovement * Speed * Time.fixedDeltaTime);
    }


    public void TestCalculationOfPath()
    {
        TestPath = CalculatePath(StartingCell, EndingCell);
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
            currentTargetCell = PlayerTarget.currentCell;
            time += Time.fixedDeltaTime;
            if(time > TimeInChaseState)
            {
                time = 0f;
                state = State.Scatter;
            }
        }

        EndingCell = currentTargetCell;

    }

    private void UpdateLine()
    {
        VisualOfPath.positionCount = TestPath.Count + 1;
        for (int i = 0; i < TestPath.Count; i++)
        {
            VisualOfPath.SetPosition(i + 1, TestPath[i].position);
        }
        VisualOfPath.SetPosition(0, StartingCell.gameObject.transform.position);
    }
    private List<Transform> CalculatePath(CellStats start, CellStats end)
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
            
            List<CellStats> neighbors;
            neighbors = GridScript.GetNeighborsOfCell(currentNode);

            foreach (CellStats neighbor in neighbors)
            {
                if(!GridScript.IsWalkable(GridScript.GetCoordsOfCell(neighbor).Item1, GridScript.GetCoordsOfCell(neighbor).Item2) || closedSet.Contains(neighbor))
                {
                    
                    continue;
                }

                float cost = currentNode.G + HeuristicCostEstimate(currentNode, neighbor);
                if(cost < neighbor.G || !openSet.Contains(neighbor))
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
            if(StartingCell == EndingCell)
            {
                Debug.Log("Target Reached!");
                //reachedObjective = true;
                if(state == State.Scatter)
                {
                    CurrentScatterIndex++;
                    if (CurrentScatterIndex == ScatterPosition.Length)
                    {
                        CurrentScatterIndex = 0;
                    }
                }

                else if(state == State.Chase)
                {
                    state = State.Scatter;
                    //return;
                }
                CalculateTarget();
                //return;
            }
            TestCalculationOfPath();
            currentTarget = TestPath[0];
            UpdateCurrentMovementDirection();
        }
    }
}
