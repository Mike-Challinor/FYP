using UnityEngine;
using System.Collections;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] private GameObject m_playerCam;
    [SerializeField] private GameObject m_player;
    [SerializeField] private bool m_isAttacking;
    [SerializeField] private float m_attackTimer = 0.2f;
    [SerializeField] private GameObject m_projectilePrefab;
    [SerializeField] private GameObject m_firePoint;

    private float m_maxHealth = 100.0f;
    private Health_Component m_healthComponent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialise player camera
        m_playerCam = GameObject.FindGameObjectWithTag("MainCamera"); // Find the main camera game object
        m_playerCam.transform.SetParent(m_player.transform); // Set the player as the cameras new parent
        m_playerCam.transform.localPosition = new Vector3(0, 0, -11); ; // Reset locallocal position

        //Initialise health component
        m_healthComponent = GetComponent<Health_Component>();
        m_healthComponent.InitHealth(m_maxHealth);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitiateAttack()
    {
        m_isAttacking = true;
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        while(m_isAttacking)
        {
            // Spawn projectile here
            GameObject projectile = Instantiate(m_projectilePrefab, m_firePoint.transform.position, m_firePoint.transform.rotation);

            // Wait for attack timer before spawning another projectile
            yield return StartCoroutine(AttackTimer());
        }
        
    }

    private IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(m_attackTimer);
    }

    public void EndAttack()
    {
        m_isAttacking = false;
    }    
}
