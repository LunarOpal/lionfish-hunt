using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 2.5f;
    private PlayerMovement movement;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // left mouse button
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        // transform mouse position from screen to world coordinates
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 

        // get mouse position to see if it hits a collider
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider != null)
        {
            Lionfish lionfish = hit.collider.GetComponent<Lionfish>();
            if (lionfish != null)
            {
                Vector2 directionToFish = lionfish.transform.position - transform.position;
                if (directionToFish.magnitude <= attackRange &
                    directionToFish.y < 0.6*attackRange & directionToFish.y > -0.6*attackRange &
                    movement.FacingDirection * directionToFish.x > 0) // same direction
                {
                    // TODO attack animation here
                    lionfish.Die();
                }

            }
        }


    }
}
