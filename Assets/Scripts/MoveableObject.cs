using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class MoveableObject : MonoBehaviour
{

    /*
     https://www.reddit.com/r/Unity3D/comments/2i97zn/how_to_get_pacman_like_constant_movement_with/ 

Store the current move direction and the latest intended move direction separately. User input goes directly to the intended move, but current move direction is what applies to the player character. Any time the player crosses a grid center, check if the latest intended move would be legal.

If the intended move direction checks out, the player is re-aligned to the grid and the current move direction is updated. If you go for a certain amount of time without putting in any input (and the intended move did not succeed), it clears.

     
     
     */

    [Header("Movement Fields")]
    public Vector2 CurrentDirection = Vector2.right;
    public Vector2 IntendedDirection = Vector2.right;
    public float MovementSpeed;
    private float _currentMoveSpeed;

    [Header("Outside Scripts")]
    public InputSystem_Actions ISAs;
    public Griddy GridReference;
    public CellStats currentCell;
    private Rigidbody2D _rb2D;

    //Input Actions
    private InputAction _right;
    private InputAction _left;
    private InputAction _up;
    private InputAction _down;
    private void Awake()
    {
        ISAs = new InputSystem_Actions();
        _currentMoveSpeed = MovementSpeed;
        _rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        /*Vector2 new_XY_Pos = CurrentDirection * _currentMoveSpeed * Time.fixedDeltaTime;
        transform.position += new Vector3(new_XY_Pos.x, new_XY_Pos.y, 0);*/
        _rb2D.linearVelocity = CurrentDirection * MovementSpeed;
    }

    private void MoveRight(InputAction.CallbackContext cxt)
    {
        IntendedDirection = Vector2.right;
        
    }

    private void MoveLeft(InputAction.CallbackContext cxt)
    {
        IntendedDirection = Vector2.left;
        
    }

    private void MoveUp(InputAction.CallbackContext cxt)
    {
        IntendedDirection = Vector2.up;
        
    }

    private void MoveDown(InputAction.CallbackContext cxt)
    {
        IntendedDirection = Vector2.down;
        
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Grid"))
        {
            currentCell = collision.gameObject.GetComponent<CellStats>();
            TestForNewDirection();

            //Debug.Log("Grid!");
        }

        else if(collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            Debug.Log("HIT TERRAIN");
            //_currentMoveSpeed = 0;
            //transform.position = currentCell.transform.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            Debug.Log("Hit hard terrain!");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            TestForNewDirection();
        }
    }



    private void TestForNewDirection()
    {
        Tuple<int, int> coordsOfNewCell;
        if (IntendedDirection == Vector2.right)
        {
            coordsOfNewCell = GridReference.AddTuples(GridReference.GetCoordsOfCell(currentCell), new Tuple<int, int>(0, 1));
        }

        else if(IntendedDirection == Vector2.left)
        {
            coordsOfNewCell = GridReference.AddTuples(GridReference.GetCoordsOfCell(currentCell), new Tuple<int, int>(0, -1));
        }

        else if(IntendedDirection == Vector2.up)
        {
            coordsOfNewCell = GridReference.AddTuples(GridReference.GetCoordsOfCell(currentCell), new Tuple<int, int>(1, 0));
        }

        else 
        {
            coordsOfNewCell = GridReference.AddTuples(GridReference.GetCoordsOfCell(currentCell), new Tuple<int, int>(-1, 0));
        }


        if(IntendedDirection != CurrentDirection && GridReference.IsWalkable(coordsOfNewCell.Item1, coordsOfNewCell.Item2))
        {
            _currentMoveSpeed = MovementSpeed;
            transform.position = currentCell.gameObject.transform.position;
            //Debug.Log("Hello here");
            CurrentDirection = IntendedDirection;
        }


    }





}
