using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;


public class GenerateGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private GameObject _gridPrefab;
    [SerializeField] private float _gridBoxSize;
    [SerializeField] private Transform _gridPointsParent;
    [SerializeField] private BoxCollider2D _spawnBounds;
    [SerializeField] private Transform _gridOrigin;

    public CellStats[,] cellGrid;

    /// <summary>
    /// Generate a new grid within the bounds of the box collider, Will generate cells as children of a specified transform and store them within a list
    /// </summary>
    public void GenerateNewGrid()
    {
        //To do: add in a way to delete the entire grid (go back to idea of having a cell holder transform that's a child of the parent transform
        ClearGrid();


        //get the sizes of the box bounds and calculate the number of rows and columns
        Vector2 boxBounds = _spawnBounds.size;
        int numRows = Mathf.FloorToInt(boxBounds.y / _gridBoxSize);
        int numCols = Mathf.FloorToInt(boxBounds.x / _gridBoxSize);
        cellGrid = new CellStats[numRows, numCols];

        //calculate origin of the box
        //origin == transform.position - boxSize / 2
        Vector2 origin = (Vector2)_gridOrigin.position - boxBounds / 2;
        origin += Vector2.one * (_gridBoxSize / 2f);

        Vector2 newSpawnPosition = Vector2.zero;
        GameObject newGridPoint = null;
        //for each coordinate:
        for (int y = 0; y < numRows; y++)
        {
            for(int x = 0; x < numCols; x++)
            {
                //calculate the position of the new point
                newSpawnPosition = origin + new Vector2(x * _gridBoxSize, y * _gridBoxSize);
                //spawn a grid point there and name it
                newGridPoint = Instantiate(_gridPrefab, newSpawnPosition, Quaternion.identity, _gridPointsParent);
                newGridPoint.name = $"Cell ({y},{x})";
                //add that object to the 2D array of grid points
                cellGrid[y, x] = newGridPoint.GetComponent<CellStats>();

            }
        }

        
    }

    /// <summary>
    /// Clear the Grid of the old cells when the box has been resized
    /// </summary>
    public void ClearGrid()
    {
        List<GameObject> toDestroy = new();

        //cache the grid points in a list of GameObjects to make sure they all get deleted
        for(int i = 0; i < _gridPointsParent.childCount; i++)
        {
            toDestroy.Add(_gridPointsParent.GetChild(i).gameObject);
        }

        foreach(GameObject dest in toDestroy)
        {
            SmartDestroy(dest);
        }

        toDestroy.Clear();
        
    }

    /// <summary>
    /// Destroy a single grid object in the scene 
    /// </summary>
    /// <param name="obj"></param>
    public static void SmartDestroy(UnityEngine.Object obj)
    {
        if (obj == null)
        {
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            GameObject.DestroyImmediate(obj);
        }
#endif
        
    }

    //to add:
    //1. helper method to find the x and y coord of a given cell stat
    public CellStats GetCellData(int xCoord, int yCoord)
    {
        if(!IsValidCoordinate(xCoord, yCoord))
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
        if(!Int32.TryParse(cellName.Substring(indexOfLeftParenthesis + 1, indexOfComma - indexOfLeftParenthesis - 1), out xCoordinate))
        {
            xCoordinate = -1;
        }

        int yCoordinate = -1;
        if(!Int32.TryParse(cellName.Substring(indexOfComma + 1, indexOfRightParenthesis - indexOfComma - 1), out yCoordinate))
        {
            yCoordinate = -1;
        }
        ;

        if(!IsValidCoordinate(xCoordinate, yCoordinate))
        {
            Debug.Log($"Cell found, but not at a valid coordinate: {xCoordinate}, {yCoordinate}");
        }

        return new Tuple<int, int>(xCoordinate, yCoordinate);
        
    }

    public List<CellStats> GetNeighborsOfCell(CellStats cell)
    {
        Tuple<int, int> coordinatesOfCell = GetCoordsOfCell(cell); 
        if(!IsValidCoordinate(coordinatesOfCell.Item1, coordinatesOfCell.Item2))
        {
            Debug.Log($"cell is not a valid coordinate: {coordinatesOfCell.Item1}, {coordinatesOfCell.Item2}");
            return null;
        }

        Debug.Log($"Current Cell?: {coordinatesOfCell.Item1}, {coordinatesOfCell.Item2}");
        List<CellStats> answer = new();
        Tuple<int, int>[] dxdy = { new Tuple<int, int>(1,0),
                                   new Tuple<int, int>(-1,0),
                                   new Tuple<int, int>(0,1),
                                   new Tuple<int, int>(0,-1)};

        string debugStatement = "";
        for(int i = 0; i < dxdy.Length; i++)
        {
            Tuple<int, int> prospectiveCoordinate = AddTuples(dxdy[i], coordinatesOfCell);
            if(IsValidCoordinate(prospectiveCoordinate.Item1, prospectiveCoordinate.Item2))
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
        if(!IsValidCoordinate(xCoord, yCoord))
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
