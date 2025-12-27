using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class MoveableObject : MonoBehaviour
{
    
    

    //Outside Scripts
    public InputSystem_Actions ISAs;
    public Griddy GridReference;
    public CellStats currentCell;

    //Input Actions
    private InputAction _right;
    private InputAction _left;
    private InputAction _up;
    private InputAction _down;
    private void Awake()
    {
        ISAs = new InputSystem_Actions();
    }

    private void Start()
    {
        transform.position = currentCell.transform.position;
    }

    private void MoveRight(InputAction.CallbackContext cxt)
    {
        Tuple<int, int> possibleMoveCoordinates = GridReference.AddTuples(GridReference.GetCoordsOfCell(currentCell), new Tuple<int, int>(0, 1));
        if (GridReference.IsWalkable(possibleMoveCoordinates.Item1, possibleMoveCoordinates.Item2))
        {
            currentCell = GridReference.GetCellData(possibleMoveCoordinates.Item1, possibleMoveCoordinates.Item2);
            transform.position = currentCell.transform.position;
        }
    }

    private void MoveLeft(InputAction.CallbackContext cxt)
    {
        Tuple<int, int> possibleMoveCoordinates = GridReference.AddTuples(GridReference.GetCoordsOfCell(currentCell), new Tuple<int, int>(0, -1));
        if (GridReference.IsWalkable(possibleMoveCoordinates.Item1, possibleMoveCoordinates.Item2))
        {
            currentCell = GridReference.GetCellData(possibleMoveCoordinates.Item1, possibleMoveCoordinates.Item2);
            transform.position = currentCell.transform.position;
        }
    }

    private void MoveUp(InputAction.CallbackContext cxt)
    {
        Tuple<int, int> possibleMoveCoordinates = GridReference.AddTuples(GridReference.GetCoordsOfCell(currentCell), new Tuple<int, int>(1, 0));
        if (GridReference.IsWalkable(possibleMoveCoordinates.Item1, possibleMoveCoordinates.Item2))
        {
            currentCell = GridReference.GetCellData(possibleMoveCoordinates.Item1, possibleMoveCoordinates.Item2);
            transform.position = currentCell.transform.position;
        }
    }

    private void MoveDown(InputAction.CallbackContext cxt)
    {
        Tuple<int, int> possibleMoveCoordinates = GridReference.AddTuples(GridReference.GetCoordsOfCell(currentCell), new Tuple<int, int>(-1, 0));
        if (GridReference.IsWalkable(possibleMoveCoordinates.Item1, possibleMoveCoordinates.Item2))
        {
            currentCell = GridReference.GetCellData(possibleMoveCoordinates.Item1, possibleMoveCoordinates.Item2);
            transform.position = currentCell.transform.position;
        }
    }

    private void OnEnable()
    {
        _right = ISAs.Player.Right;
        _left = ISAs.Player.Left;
        _up = ISAs.Player.Up;
        _down = ISAs.Player.Down;

        _right.Enable();
        _left.Enable();
        _up.Enable();
        _down.Enable();

        _right.performed += MoveRight;
        _left.performed += MoveLeft;
        _up.performed += MoveUp;
        _down.performed += MoveDown;
    }




}
