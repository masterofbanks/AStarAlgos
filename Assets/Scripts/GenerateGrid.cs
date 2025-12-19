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

    private readonly List<GameObject> cells = new();


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


        //calculate origin of the box
        //origin == transform.position - boxSize / 2
        Vector2 origin = (Vector2)_gridOrigin.position - boxBounds / 2;
        origin += Vector2.one * (_gridBoxSize / 2f);

        Vector2 newSpawnPosition = Vector2.zero;
        GameObject newGridPoint = null;
        //for each coordinate:
        for (int y = 0; y < numRows + 1; y++)
        {
            for(int x = 0; x < numCols + 1; x++)
            {
                //calculate the position of the new point
                newSpawnPosition = origin + new Vector2(x * _gridBoxSize, y * _gridBoxSize);
                //spawn a grid point there and name it
                newGridPoint = Instantiate(_gridPrefab, newSpawnPosition, Quaternion.identity, _gridPointsParent);
                newGridPoint.name = $"Cell ({x},{y})";
                //add that object to the list of grid points
                cells.Add(newGridPoint);

            }
        }

        
    }

    /// <summary>
    /// Clear the Grid of the old cells when the box has been resized
    /// </summary>
    public void ClearGrid()
    {
        List<GameObject> toDestroy = new();


        for(int i = 0; i < _gridPointsParent.childCount; i++)
        {
            toDestroy.Add(_gridPointsParent.GetChild(i).gameObject);
        }

        foreach(GameObject dest in toDestroy)
        {
            SmartDestroy(dest);
        }

        toDestroy.Clear();
        cells.Clear();
    }

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
        else
#endif
        {
            GameObject.Destroy(obj);
        }
    }

}
