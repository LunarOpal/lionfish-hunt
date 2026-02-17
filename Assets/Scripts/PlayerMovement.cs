using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    public int FacingDirection { get; private set; } = 1; // 1 for right, -1 for left

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        //check facing direction
        if (movement.x > 0)
        {
            FacingDirection = 1; // facing right
            transform.localScale = new Vector3(-1, 1, 1); // negative because default is facing left
        }
        else if (movement.x < 0)
        {
            FacingDirection = -1; // facing left
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}
