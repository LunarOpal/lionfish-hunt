using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Transform AttackPoint;
    public float attackRange = 2.5f;
    public Animator anim;
    public LayerMask lionfishLayer;
    public AudioClip stab;
    public AudioClip stabHit;

    private AudioSource audioSource;
    private PlayerMovement movement;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // don't allow attacking until the attack phase of the tutorial or after during gameplay
        if (movement.getTutorialPhase() & movement.getCurrentStep() != 1)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (movement.getIsAttacking() == false) {
                //TryAttackKey(); called during animation play
                anim.SetTrigger("Attack");
                movement.setIsAttacking(true);
            }
        }
    }


    void TryAttackKey()
    {
        Collider2D hitEnemy = Physics2D.OverlapCircle(AttackPoint.position, attackRange, lionfishLayer);

        if (hitEnemy != null)
        {
            audioSource.PlayOneShot(stabHit);
            hitEnemy.GetComponent<Lionfish>().Die();
        } else
        {
            audioSource.PlayOneShot(stab); // not hit
        }


    }

    // called during last frame of animation
    public void EndAttack()
    {
        Debug.Log("stop attacking");
        movement.setIsAttacking(false);
    }

    // for development purposes, to visualize the attack range
    void OnDrawGizmosSelected()
    {
        if (AttackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackPoint.position, attackRange);
        
    }
}
