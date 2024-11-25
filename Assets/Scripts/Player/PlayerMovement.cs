using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float horizontalInput;
    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D boxCollider;
    [SerializeField]private float speed;
    [SerializeField]private float jumpPower;
    [SerializeField]private LayerMask groundLayer;

    //Controls
    private bool moveLeft;
    private bool canMove;

    private void Awake(){
        //Inicializar as variáveis com referencia aos componentes
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        canMove = false;
    }

    private void Update(){
        //Verificar se o player está no chão
        animator.SetBool("Grounded", isGrounded());
        
        HandleMovement();
    }

    private void HandleMovement(){
        if(canMove){
            if(moveLeft){
                MoveLeft();
            }else if(!moveLeft){
                MoveRight();
            }
        }else{
            Stopmovement();
        }
    }

    public void AllowMovement(bool movement){
        canMove = true;
        moveLeft = movement;
        animator.SetBool("Run", true);
    }

    public void CancelMovement(){
        canMove = false;
        animator.SetBool("Run", false);
    } 

    public void MoveLeft(){
        transform.localScale = new Vector3(-1, 1, 1);
        body.velocity = new Vector2(speed * -1, body.velocity.y);
    }

    public void MoveRight(){
        transform.localScale = Vector3.one;
        body.velocity = new Vector2(speed, body.velocity.y);
    }

    public void Jump(){
        if(isGrounded()){
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            animator.SetTrigger("Jump");
        }
    }

    private void Stopmovement(){
        body.velocity = new Vector2(0, body.velocity.y);
    }

    // Verifica se o player está no chão (se sim, true, se não, false)
    private bool isGrounded(){
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack(){
        return horizontalInput == 0 && isGrounded();
    }

}
