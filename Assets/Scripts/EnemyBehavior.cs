using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
public class EnemyBehavior : MonoBehaviour
{
    [Header("Walkable Grid")]
    public GenerateGrid Grid;
    public Transform[] TestPath;
    public Transform currentTarget;
    public int CurrentTargetIndex;
    private Transform currentCell;

    [Header("Physics values")]
    public float Speed;
    public float epsilonDistance;

    [Header("Test of Path Construction")]
    public List<GameObject> ConstructedPath;
    public CellStats StartingCell;
    public CellStats EndingCell;

    Rigidbody2D _rb2D;
    bool reachedObjective = false;
    //
    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentTargetIndex = 0;
        currentTarget = TestPath[CurrentTargetIndex];
        currentCell = currentTarget; //start the enemy at the first cell in the path
    }

    private void FixedUpdate()
    {
        if (!reachedObjective && TestPath.Length > 0)
        {
            MoveToPoint(currentTarget);
            if (DetermineReachedPosition(currentTarget))
            {
                currentCell = currentTarget;
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

    public void TestCalculationOfPath()
    {
        ConstructedPath = CalculatePath(StartingCell, EndingCell);
        ConstructedPath.Reverse();
        TestPath = new Transform[ConstructedPath.Count + 1];
        TestPath[0] = StartingCell.transform;
        for(int i = 0; i < ConstructedPath.Count; i++)
        {
            TestPath[i + 1] = ConstructedPath[i].transform;
        }
    }
    private List<GameObject> CalculatePath(CellStats start, CellStats end)
    {
        List<GameObject> answer = new();
        List<CellStats> openSet = new();
        List<CellStats> closedSet = new();
        openSet.Add(start);
        CellStats currentNode = null;

        while(openSet.Count > 0)
        {
            openSet.Sort(Comparer<CellStats>.Create((s1, s2) => s1.F.CompareTo(s2.F)));
            currentNode = openSet[0];
            Debug.Log(currentNode.gameObject.name);
            openSet.RemoveAt(0);

            closedSet.Add(currentNode);

            //if the current node is the end goal, add each node to the answer list by iterating through the parents of each node
            if(currentNode == end)
            {
                while(currentNode != start)
                {
                    answer.Add(currentNode.gameObject);
                    currentNode = currentNode.parent;
                }

                return answer;
            }

            List<CellStats> neighbors = Grid.GetNeighborsOfCell(currentNode);
            foreach(CellStats neighbor in neighbors)
            {
                if(!Grid.IsWalkable(Grid.GetCoordsOfCell(neighbor).Item1, Grid.GetCoordsOfCell(neighbor).Item2) || closedSet.Contains(neighbor))
                {
                    
                    continue;
                }

                /*Debug.Log($"Did not Continued on {Grid.GetCoordsOfCell(neighbor).Item1}, {Grid.GetCoordsOfCell(neighbor).Item2}");*/
                int cost = currentNode.G + HeuristicCostEstimate(currentNode, neighbor);
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

    private int HeuristicCostEstimate(CellStats cell1, CellStats cell2)
    {
        Tuple<int, int> cellOneCoords = Grid.GetCoordsOfCell(cell1);
        Tuple<int, int> cellTwoCoords = Grid.GetCoordsOfCell(cell2);

        return Mathf.Abs(cellTwoCoords.Item1 - cellOneCoords.Item1) + Mathf.Abs(cellTwoCoords.Item2 - cellOneCoords.Item1);

    }
}
