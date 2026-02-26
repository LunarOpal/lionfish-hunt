using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public int FacingDirection { get; private set; } = 1; // 1 for right, -1 for left
    public Animator anim;
    public float minX = -6f; // minimum x position
    public float maxX = 6f; // maximum x position
    public float minY = -4f; // minimum y position
    public float maxY = 4f; // maximum y position


    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isAttacking = false;

    private GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // if not attacking animation, then can move/change direction
        // while attacking, refrain from changing direction
        if (isAttacking == false & gameManager.getGameEnd() == false) {
            
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
        if (isAttacking == false)
        {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }

    }

    // make sure player does not move out of bounds
    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        
        // set to spotlight radius in tutorial
        if (gameManager.getTutorialPhase())
        {
            pos.x = Mathf.Clamp(pos.x, -0.8f, 0.8f);
            pos.y = Mathf.Clamp(pos.y, 0.8f, 2.4f);
        } else
        {
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }

        transform.position = pos;
    }

    public bool getIsAttacking()
    {
        return isAttacking;
    }

    public void setIsAttacking(bool attackBool)
    {
        isAttacking = attackBool;
    }
}
