using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float m_projectileSpeed = 150f;
    [SerializeField] private float m_damage = 40f;
    [SerializeField] private float m_lifespan = 4f;
    private Rigidbody2D m_RB;

    void Start()
    {
        DestroyProjectileAfterLifespan();
        m_RB = GetComponent<Rigidbody2D>();
        m_RB.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Update()
    {
        MoveProjectile();
    }

    // Detect collision with walls
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision!");

        if (collision.gameObject.CompareTag("Walls"))
        {
            Debug.Log("Projectile collided with wall!");
            DestroyProjectile();
        }

        else if (collision.gameObject.CompareTag("Props"))
        {
            Debug.Log("Projectile collided with Prop!");
            DestroyProjectile();
        }

        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Projectile collided with Enemy!");
            DestroyProjectile();
            Health_Component healthComponent = collision.gameObject.GetComponentInParent<Health_Component>();
            healthComponent.RemoveHealth(m_damage);
        }
    }

    public void SetDirection(Vector2 fireDirection)
    {
        float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }
    public void SetDamage(float damage)
    {
        m_damage = damage;
    }

    private void MoveProjectile()
    {
        m_RB.linearVelocity = transform.up * m_projectileSpeed;
    }

    void DestroyProjectileAfterLifespan()
    {
        StartCoroutine(LifespanTimer());
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    IEnumerator LifespanTimer()
    {
        yield return new WaitForSeconds(m_lifespan);
        DestroyProjectile();
    }
}