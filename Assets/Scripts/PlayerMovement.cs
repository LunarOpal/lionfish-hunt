using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    public int FacingDirection { get; private set; } = 1; // 1 for right, -1 for left

    public Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // if not attacking animation, then can move/change direction
        // while attacking, refrain from changing direction
        if (anim.GetBool("isAttacking") == false) {
            
            //check facing direction
            if (movement.x > 0)
            {
                FacingDirection = 1; // facing right
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * -1;
                transform.localScale = scale;
            }
            else if (movement.x < 0)
            {
                FacingDirection = -1; // facing left
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
        }
    } 

    void FixedUpdate()
    {
        if (anim.GetBool("isAttacking") == false)
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }

    }
}
