using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform AttackPoint;
    public float attackRange = 2.5f;
    public Animator anim;
    public LayerMask lionfishLayer;
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
        //if (Input.GetMouseButtonDown(0)) // left mouse button
        //{
        //    TryAttack();
        //}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryAttackKey();
            anim.SetBool("isAttacking", true);
        }
    }

    // mouse attack deprecated
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

    void TryAttackKey()
    {
        Collider2D hitEnemy = Physics2D.OverlapCircle(AttackPoint.position, attackRange, lionfishLayer);

        if (hitEnemy != null)
        {
            hitEnemy.GetComponent<Lionfish>().Die();
        }

    }

    
    public void EndAttack()
    {
        anim.SetBool("isAttacking", false);
    }

    // for development purposes, to visualize the attack range
    void OnDrawGizmosSelected()
    {
        if (AttackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackPoint.position, attackRange);
        
    }
}
