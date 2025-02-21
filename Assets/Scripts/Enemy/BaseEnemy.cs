using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BaseEnemy : MonoBehaviour
{
    [SerializeField] private float m_moveSpeed = 1f;
    [SerializeField] private float m_attackDamage = 10f;
    [SerializeField] private float m_attackRange = 3f;
    [SerializeField] private float m_attackDuration = 1f;
    [SerializeField] private float m_attackCooldown = 2f;
    [SerializeField] private float m_aggroRange = 3f;
    [SerializeField] private GameObject m_target;
    [SerializeField] private BoxCollider2D m_attackCollider;
    [SerializeField] private bool m_isAttacking = false;

    private bool m_isFacingRight = false;
    private SpriteRenderer m_enemySprite;

    private Rigidbody2D m_RB;
    private Health_Component m_healthComponent;

    private NavMeshAgent m_navMeshAgent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_RB = GetComponent<Rigidbody2D>();
        m_healthComponent = GetComponent<Health_Component>();
        m_enemySprite = GetComponent<SpriteRenderer>();

        // Initialise the nav mesh agent and updates 
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_navMeshAgent.updateRotation = false;
        m_navMeshAgent.updateUpAxis = false;

        m_healthComponent.InitHealth(100);

        if (Random.Range(0, 100) < 50)
        {
            FlipSprite();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Reset move direction
        Vector2 moveDir = Vector2.zero;

        // Look for a target if the enemy does not have one
        if (m_target == null)
        {
            FindTarget();
        }
        else
        {
            // Set the enemy's move direction towards the target
            moveDir = m_target.transform.position - transform.position;

            // Normalize the move direction
            moveDir.Normalize();

            // Calculate distance from target
            float distance = Vector2.Distance(m_target.transform.position, transform.position);

            if (distance <= m_aggroRange) // Only move towards the player if the player is within aggro range
            {
                if (distance > m_attackRange)
                {
                    /* // Move the enemy towards the target
                    m_RB.linearVelocity = moveDir * m_moveSpeed; */

                    m_navMeshAgent.SetDestination(m_target.transform.position);

                    if ((moveDir.x > 0 && !m_isFacingRight) || (moveDir.x < 0 && m_isFacingRight))
                    {
                        // If sprite is facing the wrong way, flip the sprite
                        FlipSprite();
                    }
                }
                else
                {
                    // Initiate enemy attack if not already attacking
                    if (!m_isAttacking)
                    {
                        StartCoroutine(InitiateAttack());
                    }
                }
            }

            else
            {
                m_RB.linearVelocity = Vector2.zero; // Stop moving the enemy when out of range
            }
            
        }
    }

    // Function for finding the enemy's target
    private void FindTarget()
    {
        // Find the player and set as target
        m_target = GameObject.FindGameObjectWithTag("Player");
    }

    // Begin attack which calls the attack timer
    private IEnumerator InitiateAttack()
    {
        m_isAttacking = true;
        yield return StartCoroutine(AttackTimer()); // Wait for attack timer to finish before continuing
        AttackTarget();
    }

    // Attack timer that signifies how long the attack/animation takes
    private IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(m_attackDuration);
    }

    // Attack the target once the timer has ended
    private void AttackTarget()
    {
        // Define the attack area based on the collider's center
        Vector2 attackCenter = m_attackCollider.bounds.center;
        Vector2 attackSize = new Vector2(m_attackCollider.bounds.size.x, m_attackCollider.bounds.size.y);

        // Call the attack target funciton
        AttackTarget(attackCenter, attackSize);
    }

    private void AttackTarget(Vector2 attackCenter, Vector2 attackSize)
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(attackCenter, attackSize, 0);
        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Player")) // Ensure the collider belongs to the player
            {
                Health_Component playerHealth = collider.GetComponentInParent<Health_Component>();

                if (playerHealth != null)
                {
                    // Apply damage on the server
                    playerHealth.RemoveHealth(m_attackDamage);

                }

                else
                {
                    Debug.LogError("ENEMYCONTROLLER::ATTACKTARGETRPC:: Health component is null");
                }
            }
        }

        // Call attack cooldown after applying damage
        StartCoroutine(AttackCooldown());
    }

    // Timer for attack cooldown (time it takes between attacks)
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(m_attackCooldown);
        m_isAttacking = false;
    }

    // Function for flipping the sprite
    void FlipSprite()
    {
        m_isFacingRight = !m_isFacingRight;
        m_enemySprite.flipX = !m_enemySprite.flipX;

        // Move collider position to correct direction
        if (m_isFacingRight)
        {
            m_attackCollider.offset = new Vector2(Mathf.Abs(m_attackCollider.offset.x), m_attackCollider.offset.y);
        }
        else
        {
            m_attackCollider.offset = new Vector2(-Mathf.Abs(m_attackCollider.offset.x), m_attackCollider.offset.y);
        }
    }

    // Debug function for drawing gizmos of the enemy's attack size
    private void OnDrawGizmos()
    {
        if (m_attackCollider != null)
        {
            Vector2 attackCenter = m_attackCollider.bounds.center;
            Vector2 attackSize = new Vector2(m_attackCollider.bounds.size.x, m_attackCollider.bounds.size.y);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(attackCenter, (Vector3)attackSize); // Cast to Vector3 for compatibility

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, m_aggroRange);
        }
        else
        {
            Debug.LogWarning("m_attackCollider is null. Gizmos for attack area will not be drawn.");
        }
    }

}
