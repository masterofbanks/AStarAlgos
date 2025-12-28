using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
public class EnemyBehavior : MonoBehaviour
{
    [Header("Walkable Grid")]
    public Griddy GridScript;
    public List<Transform> TestPath;
    public Transform currentTarget;
    public int CurrentTargetIndex;

    [Header("Physics values")]
    public float Speed;
    public float epsilonDistance;

    [Header("Path Construction")]
    public CellStats StartingCell;
    public LineRenderer VisualOfPath;
    public MoveableObject PlayerTarget;

    private CellStats EndingCell;

    Rigidbody2D _rb2D;
    bool reachedObjective = false;
    bool firstPathCalc = true;
    //
    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EndingCell = PlayerTarget.currentCell;
        TestCalculationOfPath();
        currentTarget = TestPath[0];
        firstPathCalc = false;
        
    }

    private void FixedUpdate()
    {
        EndingCell = PlayerTarget.currentCell;
        if (!reachedObjective)
        {
            if (currentTarget == null)
            {
                Debug.Log("Current Target is null");
            }
            MoveToPoint(currentTarget);
        }

        //TestCalculationOfPath();
            
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

    public void TestCalculationOfPath()
    {
        List<Transform> newPath = CalculatePath(StartingCell, EndingCell);
        TestPath = newPath;

        UpdateLine();



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
        CellStats currentNode = null;

        while(openSet.Count > 0)
        {
            openSet.Sort(Comparer<CellStats>.Create((s1, s2) => s1.F.CompareTo(s2.F)));
            currentNode = openSet[0];
            //Debug.Log(currentNode.gameObject.name);
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

            List<CellStats> neighbors = GridScript.GetNeighborsOfCell(currentNode);
            foreach(CellStats neighbor in neighbors)
            {
                if(!GridScript.IsWalkable(GridScript.GetCoordsOfCell(neighbor).Item1, GridScript.GetCoordsOfCell(neighbor).Item2) || closedSet.Contains(neighbor))
                {
                    
                    continue;
                }

                /*Debug.Log($"Did not Continued on {Grid.GetCoordsOfCell(neighbor).Item1}, {Grid.GetCoordsOfCell(neighbor).Item2}");*/
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Grid"))
        {
            //Debug.Log("Hit new grid point");
            StartingCell = collision.gameObject.GetComponent<CellStats>();
            if(StartingCell == EndingCell)
            {
                Debug.Log("Target Reached!");
                reachedObjective = true;
                return;
            }
            TestCalculationOfPath();
            currentTarget = TestPath[0];
        }
    }
}
