using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D boxCollider;
    
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    private Vector3 startingPosition;

    // Controls
    private bool moveLeft;
    private bool canMove;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        canMove = false;

    }

    private void Start()
    {
        Time.timeScale = 1f;
        startingPosition = new Vector3(-19, -12, 0);
        ResetPlayer();
    }

    private void Update()
    {
        animator.SetBool("Grounded", isGrounded());
        
        HandleMovement();

        // Handle Jump input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            Jump();
        }
    }

    private void HandleMovement()
    {
        if (canMove || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (moveLeft || Input.GetKey(KeyCode.A))
            {
                MoveLeft();
            }
            else if (!moveLeft || Input.GetKey(KeyCode.D))
            {
                MoveRight();
            }
        }
        else
        {
            StopMovement();
        }
    }

    public void AllowMovement(bool movement)
    {
        canMove = true;
        moveLeft = movement;
        animator.SetBool("Run", true);
    }

    public void CancelMovement()
    {
        canMove = false;
        animator.SetBool("Run", false);
    }

    public void MoveLeft()
    {
        transform.localScale = new Vector3(-1, 1, 1);

        // Only apply force if velocity is below the max speed
        if (Mathf.Abs(body.velocity.x) < speed)
        {
            body.velocity = new Vector2(-speed, body.velocity.y);
        }
    }


    public void MoveRight()
    {
        transform.localScale = Vector3.one;

        // Only apply force if velocity is below the max speed
        if (Mathf.Abs(body.velocity.x) < speed)
        {
            body.velocity = new Vector2(speed, body.velocity.y);
        }
    }


    private void Jump()
    {
        body.velocity = new Vector2(body.velocity.x, jumpPower);
        animator.SetTrigger("Jump");
    }

    private void StopMovement()
    {
        body.velocity = new Vector2(0, body.velocity.y);
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center, 
            boxCollider.bounds.size, 
            0, 
            Vector2.down, 
            0.1f, 
            groundLayer
        );
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return body.velocity.x == 0 && isGrounded();
    }

    public void ResetPlayer()
    {
        transform.position = startingPosition;
        body.velocity = Vector2.zero;
        canMove = false;
        moveLeft = false;

        // Reset animations
        animator.SetBool("Run", false);
        animator.SetBool("Grounded", true);
    }
}
