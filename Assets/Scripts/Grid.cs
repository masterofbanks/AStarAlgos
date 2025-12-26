using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private Transform _gridObjectParent;
    [SerializeField] private GenerateGrid _gridGenerator;
    private CellStats[,] cellGrid;

    private void Awake()
    {

    }

    private void Start()
    {
        Tuple<int, int> gridDimensions = _gridGenerator.GetGridDimensions();
        cellGrid = new CellStats[gridDimensions.Item1, gridDimensions.Item2];
        int i = 0;
        for(int y = 0; y < cellGrid.GetLength(0); y++)
        {
            for(int x = 0; x < cellGrid.GetLength(1); x++)
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
            Debug.Log($"There is no cell at coordinate ({xCoord}, {yCoord})");
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

        //Debug.Log($"Current Cell?: {coordinatesOfCell.Item1}, {coordinatesOfCell.Item2}");
        List<CellStats> answer = new();
        Tuple<int, int>[] dxdy = { new Tuple<int, int>(1,0),
                                   new Tuple<int, int>(-1,0),
                                   new Tuple<int, int>(0,1),
                                   new Tuple<int, int>(0,-1)};

        string debugStatement = "";
        for (int i = 0; i < dxdy.Length; i++)
        {
            Tuple<int, int> prospectiveCoordinate = AddTuples(dxdy[i], coordinatesOfCell);
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

        //add walkability later 
        //return GetCellData(xCoord, yCoord).walkable
        return true;
    }

    private bool IsValidCoordinate(int xCoord, int yCoord)
    {
        return (xCoord >= 0 && xCoord < cellGrid.GetLength(0)) && (yCoord >= 0 && yCoord < cellGrid.GetLength(1));
    }

    private Tuple<int, int> AddTuples(Tuple<int, int> t1, Tuple<int, int> t2)
    {
        //Debug.Log($"{t1.Item1 + t2.Item1}, {t1.Item2 + t2.Item2}");
        return new Tuple<int, int>(t1.Item1 + t2.Item1, t1.Item2 + t2.Item2);
    }
}
