using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PacmanBehavior : MonoBehaviour
{

    /*
     https://www.reddit.com/r/Unity3D/comments/2i97zn/how_to_get_pacman_like_constant_movement_with/ 

Store the current move direction and the latest intended move direction separately. User input goes directly to the intended move, but current move direction is what applies to the player character. Any time the player crosses a grid center, check if the latest intended move would be legal.

If the intended move direction checks out, the player is re-aligned to the grid and the current move direction is updated. If you go for a certain amount of time without putting in any input (and the intended move did not succeed), it clears.

     
     
     */

    [Header("Movement Fields")]
    public float MovementSpeed;
    public GameObject EatPelletSfx;
    public GameObject EatFruitSfx;
    public Transform PelletSFXParent;

    public Vector2 CurrentDirection { get; private set; } = Vector2.right ;
    private Vector2 IntendedDirection = Vector2.right;
    private Vector3 CurrentOrientation = Vector3.zero;
    private Vector3 IntendedOrientation = Vector3.zero;

    public InputSystem_Actions ISAs;

    [Header("Outside Scripts")]
    public Griddy GridReference;
    public GameManager GameScript;
    public CellStats currentCell;
    private Rigidbody2D _rb2D;
    private Animator anime;
    private SpriteRenderer rend;

    //Input Actions
    private InputAction _right;
    private InputAction _left;
    private InputAction _up;
    private InputAction _down;

    //important fields
    [Header("Death Stuff")]
    public AnimationClip PacDeathClip;
    public float TimeBetweenDeathAndRestart;
    public CellStats PacmanStartingCell;
    private bool _isAlive;
    private void Awake()
    {
        ISAs = new InputSystem_Actions();
        _rb2D = GetComponent<Rigidbody2D>();
        anime = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        _isAlive = true;
    }

    private void FixedUpdate()
    {
        /*Vector2 new_XY_Pos = CurrentDirection * _currentMoveSpeed * Time.fixedDeltaTime;
        transform.position += new Vector3(new_XY_Pos.x, new_XY_Pos.y, 0);*/
        if(GameScript.CharactersAreMoveable && _isAlive)
            _rb2D.linearVelocity = CurrentDirection * MovementSpeed;
        else
        {
            _rb2D.linearVelocity = Vector2.zero;
        }
    }

    private void MoveRight(InputAction.CallbackContext cxt)
    {
        IntendedDirection = Vector2.right;
        IntendedOrientation = new Vector3(0, 0, 0);
        
    }

    private void MoveLeft(InputAction.CallbackContext cxt)
    {
        IntendedDirection = Vector2.left;
        IntendedOrientation = new Vector3(0, 0, 180);

    }

    private void MoveUp(InputAction.CallbackContext cxt)
    {
        IntendedDirection = Vector2.up;
        IntendedOrientation = new Vector3(0, 0, 90);

    }

    private void MoveDown(InputAction.CallbackContext cxt)
    {
        IntendedDirection = Vector2.down;
        IntendedOrientation = new Vector3(0, 0, 270);

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

            if(currentCell.exitCell != null)
            {
                transform.position = currentCell.exitCell.transform.position;
            }
            TestForNewDirection();

        }

        else if (collision.gameObject.CompareTag("PowerPellet"))
        {
            Debug.Log($"Pacman hit {collision.gameObject.name}");
            GameScript.MakeEveryGhostScared();
            Destroy(collision.gameObject);
        }

        else if (collision.gameObject.CompareTag("NormalPellet"))
        {
            GameScript.NumberOfDots--;
            GameScript.NumberOfDotsCollected++;
            if(PelletSFXParent.childCount < 1)
            {
                Instantiate(EatPelletSfx, transform.position, Quaternion.identity, PelletSFXParent);
            }
            Destroy(collision.gameObject);
        }

        else if (collision.gameObject.CompareTag("Fruit"))
        {
            Debug.Log($"Pacman hit a {collision.gameObject.name}");
            Instantiate(EatFruitSfx, transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
        }

       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            anime.speed = 0;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            TestForNewDirection();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            anime.speed = 1;
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


        if (IntendedDirection != CurrentDirection && GridReference.IsWalkable(coordsOfNewCell.Item1, coordsOfNewCell.Item2) && GridReference.IsWalkableForPacman(coordsOfNewCell.Item1, coordsOfNewCell.Item2))
        {
            transform.position = currentCell.gameObject.transform.position;
            CurrentDirection = IntendedDirection;
            CurrentOrientation = IntendedOrientation;
            transform.eulerAngles = CurrentOrientation;
        }


    }

    public void PerformDeath()
    {
        _isAlive = false;
        GameScript.CharactersAreMoveable = false;
        anime.SetBool("alive", _isAlive);
        CircleCollider2D[] pacColliders = GetComponents<CircleCollider2D>();
        for(int i = 0; i < pacColliders.Length; i++)
        {
            pacColliders[i].enabled = false;
        }
        SoundManager.PlaySound(SoundType.DEAD, 0.065f, 0, false);
        StartCoroutine(DestroyPacman());
    }

    IEnumerator DestroyPacman()
    {
        float bufferBetweenDeath = 0.1f;
        yield return new WaitForSeconds(PacDeathClip.length + bufferBetweenDeath);
        rend.enabled = false;
        yield return new WaitForSeconds(TimeBetweenDeathAndRestart - (PacDeathClip.length + bufferBetweenDeath));
        GameScript.PerformLoseState();

    }

    public void SetPacmanAnimatorSpeed(float speed)
    {
        anime.speed = speed;
    }

    public void ResetPacman()
    {
        transform.position = PacmanStartingCell.transform.position;
        rend.enabled = true;
        CircleCollider2D[] pacColliders = GetComponents<CircleCollider2D>();
        for (int i = 0; i < pacColliders.Length; i++)
        {
            pacColliders[i].enabled = true;
        }
        _isAlive = true;
        anime.SetBool("alive", _isAlive);
        CurrentDirection = Vector2.right;
        IntendedDirection = Vector2.right;
        _rb2D.linearVelocity = CurrentDirection * MovementSpeed;
        transform.eulerAngles = new Vector3(0,0,0);
    }






}
