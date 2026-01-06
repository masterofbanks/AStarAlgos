using System;
using System.Collections.Generic;
using UnityEngine;

public class Griddy : MonoBehaviour
{
    [SerializeField] private Transform _gridObjectParent;
    [SerializeField] private GenerateGrid _gridGenerator;
    private CellStats[,] cellGrid;
    public List<CellStats> _walkableGridPoints;
    private System.Random rndGen;

    private void Awake()
    {
        Tuple<int, int> gridDimensions = _gridGenerator.GetGridDimensions();
        cellGrid = new CellStats[gridDimensions.Item1, gridDimensions.Item2];
        int i = 0;
        rndGen = new System.Random();
        _walkableGridPoints = new();
        for (int y = 0; y < cellGrid.GetLength(0); y++)
        {
            for (int x = 0; x < cellGrid.GetLength(1); x++)
            {
                cellGrid[y, x] = _gridObjectParent.GetChild(i).gameObject.GetComponent<CellStats>();
                i++;
            }
        }
    }




    //to add:
    //1. helper method to find the x and y coord of a given cell stat
    public CellStats GetCellData(int xCoord, int yCoord)
    {
        if (!IsValidCoordinate(xCoord, yCoord))
        {
            //Debug.Log($"There is no cell at coordinate ({xCoord}, {yCoord})");
            return null;
        }

        return cellGrid[xCoord, yCoord];
    }

    //2. reverse of the above
    public Tuple<int, int> GetCoordsOfCell(CellStats cell)
    {
        string cellName = cell.gameObject.name;
        int indexOfComma = cellName.IndexOf(',');
        int indexOfLeftParenthesis = cellName.IndexOf('(');
        int indexOfRightParenthesis = cellName.IndexOf(')');

        int xCoordinate = -1;
        if (!Int32.TryParse(cellName.Substring(indexOfLeftParenthesis + 1, indexOfComma - indexOfLeftParenthesis - 1), out xCoordinate))
        {
            xCoordinate = -1;
        }

        int yCoordinate = -1;
        if (!Int32.TryParse(cellName.Substring(indexOfComma + 1, indexOfRightParenthesis - indexOfComma - 1), out yCoordinate))
        {
            yCoordinate = -1;
        }
        ;

        if (!IsValidCoordinate(xCoordinate, yCoordinate))
        {
            Debug.Log($"Cell found, but not at a valid coordinate: {xCoordinate}, {yCoordinate}");
        }

        return new Tuple<int, int>(xCoordinate, yCoordinate);

    }

    public List<CellStats> GetNeighborsOfCell(CellStats cell)
    {
        Tuple<int, int> coordinatesOfCell = GetCoordsOfCell(cell);
        if (!IsValidCoordinate(coordinatesOfCell.Item1, coordinatesOfCell.Item2))
        {
            Debug.Log($"cell is not a valid coordinate: {coordinatesOfCell.Item1}, {coordinatesOfCell.Item2}");
            return null;
        }

        List<CellStats> answer = new();
        HashSet<Tuple<int, int>> dxdySet = new HashSet<Tuple<int, int>>{    new Tuple<int, int>(0,1),
                                                                            new Tuple<int, int>(0,-1),
                                                                            new Tuple<int, int>(1,0),
                                                                            new Tuple<int, int>(-1,0)};

        /*Tuple<int, int> backwardsMoveDirection = new Tuple<int, int>(-1 * currentMoveDirection.Item1, -1 * currentMoveDirection.Item2);

        //Debug.Log(backwardsMoveDirection);
        if (dxdySet.Contains(backwardsMoveDirection))
        {
            dxdySet.Remove(backwardsMoveDirection);
        }*/
        string debugStatement = "";
        foreach(Tuple<int,int> dxdyCoord in dxdySet)
        {
            Tuple<int, int> prospectiveCoordinate = AddTuples(dxdyCoord, coordinatesOfCell);
            if (IsValidCoordinate(prospectiveCoordinate.Item1, prospectiveCoordinate.Item2))
            {
                answer.Add(GetCellData(prospectiveCoordinate.Item1, prospectiveCoordinate.Item2));
                debugStatement += $"{prospectiveCoordinate.Item1}, {prospectiveCoordinate.Item2}";
            }

            else
            {
                //Debug.Log($"{prospectiveCoordinate.Item1}, {prospectiveCoordinate.Item2}");
            }
        }
        

        if(cell.exitCell != null)
        {
            answer.Add(cell.exitCell);
        }

        //Debug.Log(debugStatement);
        return answer;
    }

    //3. determing whether a given cell stat is walkable (exists within the bounds of the grid or is considered blocked 
    public bool IsWalkable(int xCoord, int yCoord)
    {
        if (!IsValidCoordinate(xCoord, yCoord))
        {
            return false;
        }

        return GetCellData(xCoord, yCoord).Walkable;
    }

    public bool IsWalkableForPacman(int xCoord, int yCoord)
    {
        if (!IsValidCoordinate(xCoord, yCoord))
        {
            return false;
        }

        return !GetCellData(xCoord, yCoord).PacmanCantWalkHere;
    }

    private bool IsValidCoordinate(int xCoord, int yCoord)
    {
        return (xCoord >= 0 && xCoord < cellGrid.GetLength(0)) && (yCoord >= 0 && yCoord < cellGrid.GetLength(1));
    }

    public Tuple<int, int> AddTuples(Tuple<int, int> t1, Tuple<int, int> t2)
    {
        //Debug.Log($"{t1.Item1 + t2.Item1}, {t1.Item2 + t2.Item2}");
        return new Tuple<int, int>(t1.Item1 + t2.Item1, t1.Item2 + t2.Item2);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <returns>t1 - t2</returns>
    public Tuple<int, int> SubTuples(Tuple<int, int> t1, Tuple<int, int> t2)
    {
        return new Tuple<int, int>(t1.Item1 - t2.Item1, t1.Item2 - t2.Item2);
    }

    public CellStats FindARandomWalkableGridPoint(CellStats currentCell, CellStats[] ScatterCells)
    {
        HashSet<CellStats> scatterSet = new();
        for(int i = 0; i < ScatterCells.Length; i++)
        {
            scatterSet.Add(ScatterCells[i]);
        }

        _walkableGridPoints.Clear();
        for (int y = 0; y < cellGrid.GetLength(0); y++)
        {
            for (int x = 0; x < cellGrid.GetLength(1); x++)
            {
                if (cellGrid[y, x].Walkable && cellGrid[y,x] != currentCell && !scatterSet.Contains(cellGrid[y,x]))
                {
                    _walkableGridPoints.Add(cellGrid[y, x]);
                }
            }
        }
        int randIndex = rndGen.Next(0, _walkableGridPoints.Count);
        return _walkableGridPoints[randIndex];
    }

    public List<Transform> FindDotSpawnLocations()
    {
        List<Transform> ans = new();
        for(int y = 0; y < cellGrid.GetLength(0); y++)
        {
            for(int x = 0; x < cellGrid.GetLength(1); x++)
            {
                if (cellGrid[y,x].Walkable && !cellGrid[y, x].DontSpawnDotHere)
                {
                    ans.Add(cellGrid[y, x].transform);
                }
            }
        }

        return ans;
    }

    public CellStats FindNearestWalkableCell(CellStats startingCell, CellStats walkable)
    {
        List<CellStats> openSet = new();
        List<CellStats> closedSet = new();
        openSet.Add(startingCell);
        CellStats currentNode = null;

        while(openSet.Count > 0)
        {
            currentNode = openSet[0];
            openSet.RemoveAt(0);
            closedSet.Add(currentNode);

            if (currentNode.Walkable)
            {
                return currentNode;
            }

            List<CellStats> neighbors = GetNeighborsOfCell(currentNode);
            foreach(CellStats neighbor in neighbors)
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
            }

            neighbors.Clear();
        }

        return walkable;
    }
}
