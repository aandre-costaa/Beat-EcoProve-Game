using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D boxCollider;
    
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private AudioClip jumpSound;
    private Vector3 startingPosition;

    // Controls
    private bool moveLeft;
    private bool canMove;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        canMove = false;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        startingPosition = new Vector3(-19.035f, -12.256f, 0);
        ResetPlayer();
    }

    private void Update()
    {
        animator.SetBool("Grounded", isGrounded());
        //animator.SetBool("Jump", !isGrounded());
        
        HandleMovement();

        // Handle Jump input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            Jump();
        }
    }

    private void HandleMovement()
    {
        bool inputLeft = Input.GetKey(KeyCode.A);
        bool inputRight = Input.GetKey(KeyCode.D);

        if (canMove || inputLeft || inputRight)
        {
            if (moveLeft || inputLeft)
                MoveLeft();
            else if (!moveLeft || inputRight)
                MoveRight();
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
        transform.localScale = new Vector3(-3, 3, 3);

        // Only apply force if velocity is below the max speed
        if (Mathf.Abs(body.velocity.x) < speed)
        {
            body.velocity = new Vector2(-speed, body.velocity.y);
        }
    }


    public void MoveRight()
    {
        transform.localScale = new Vector3(3, 3, 3);

        // Only apply force if velocity is below the max speed
        if (Mathf.Abs(body.velocity.x) < speed)
        {
            body.velocity = new Vector2(speed, body.velocity.y);
        }
    }


    private void Jump()
    {
        if (isGrounded()) {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            //animator.SetBool("Jump", true);
            animator.SetTrigger("Jump_Trigger");
            SoundManager.Instance.PlaySound(jumpSound); 
        }
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
