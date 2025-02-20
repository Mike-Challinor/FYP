using UnityEngine;
using System.Collections;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] private GameObject m_playerCam;
    [SerializeField] private GameObject m_player;
    [SerializeField] private Player_HUD m_playerHUD;
    [SerializeField] private bool m_isAttacking = false;
    [SerializeField] private bool m_canAttack = true;
    [SerializeField] private bool m_canInteract = false;
    [SerializeField] private float m_attackTimer = 0.5f;
    [SerializeField] private float m_attackDamage = 40f;
    [SerializeField] private GameObject m_projectilePrefab;
    [SerializeField] private GameObject m_firePoint;
    [SerializeField] private GameObject m_playerCameraPrefab;
    [SerializeField] private GameObject m_interactableObject;
    [SerializeField] private int m_keyCount = 0;

    private bool isCollidingWithWall = false;
    private float m_maxHealth = 100.0f;
    private Health_Component m_healthComponent;
    private Rigidbody2D m_RB;

    void Start()
    {
        Vector3 cameraPosition = new Vector3(0, 0, -10);
        SpawnPlayerCamera(cameraPosition, transform.rotation);
        m_healthComponent = GetComponent<Health_Component>();
        m_healthComponent.InitHealth(m_maxHealth);
        m_RB = GetComponent<Rigidbody2D>();
        m_playerHUD.InitHUD(m_maxHealth);
    }

    void FixedUpdate()
    {
        if (IsMouseWithinScreen() && !isCollidingWithWall)
        {
            FaceMousePos();
        }
    }

    // Detect collision with walls
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision!");

        if (collision.gameObject.CompareTag("Walls"))
        {
            Debug.Log("Player Collided with wall!");
            isCollidingWithWall = true;
        }
    }

    // Detect when collision ends
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            isCollidingWithWall = false;
        }
    }

    public void InitiateAttack()
    {
        if (IsMouseWithinScreen())
        {
            m_isAttacking = true;
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        while (m_isAttacking)
        {
            GameObject projectile = Instantiate(m_projectilePrefab, m_firePoint.transform.position, m_firePoint.transform.rotation);

            Vector3 mousePos = m_playerCam.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(m_playerCam.transform.position.z)));
            Vector2 fireDirection = (mousePos - m_firePoint.transform.position).normalized;

            projectile.GetComponent<Projectile>().SetDirection(fireDirection);
            projectile.GetComponent<Projectile>().SetDamage(m_attackDamage);

            m_canAttack = false;

            yield return StartCoroutine(AttackTimer());
        }
    }

    private IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(m_attackTimer);
        m_canAttack = true;
    }

    public void EndAttack()
    {
        m_isAttacking = false;
    }

    public bool GetCanAttack()
    {
        return m_canAttack;
    }

    private void FaceMousePos()
    {
        if (!isCollidingWithWall)
        {
            Vector3 mousePos = m_playerCam.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(m_playerCam.transform.position.z)));
            mousePos.z = 0;
            Vector3 direction = (mousePos - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 88f);
        }
    }

    private bool IsMouseWithinScreen()
    {
        Vector3 mousePos = Input.mousePosition;
        return mousePos.x >= 0 && mousePos.x <= Screen.width && mousePos.y >= 0 && mousePos.y <= Screen.height;
    }

    private void SpawnPlayerCamera(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GameObject playerCamera = Instantiate(m_playerCameraPrefab, spawnPosition, spawnRotation);

        if (playerCamera != null)
        {
            m_playerCam = playerCamera;
            playerCamera.GetComponent<FollowCamera>().InitCamera(this.transform);
        }
        else
        {
            Debug.LogError("ERROR::PLAYERNETWORK::SPAWNPLAYERCAMERA:: Player camera is null");
        }
    }

    public void Interact()
    {
        m_interactableObject.GetComponent<RewardChest>().GetReward(this);
    }

    public void SetInteractionStatus(bool canInteract, GameObject interactObject)
    {
        m_interactableObject = interactObject;
        m_canInteract = canInteract;

        if (m_canInteract)
        {
            m_playerHUD.ShowInteractText(true);
        }

        else
        {
            m_playerHUD.ShowInteractText(false);
        }
    }

    public bool GetInteractionStatus()
    {
        return m_canInteract;
    }

    public void AddKey()
    {
        m_keyCount++;
    }
    public int GetKeyCount()
    {
        return m_keyCount;
    }

    // Decrease attack timer by percentage
    public void DecreaseAttackTimer(float percentage)
    {
        // Turn percentage to a decimal
        float percentageDecimal = percentage / 100;

        // Get percentage of the attack timer
        float amountToRemove = m_attackTimer * percentageDecimal;

        // Remove percentage from the attack timer
        m_attackTimer = m_attackTimer - amountToRemove;
    }

    public void IncreaseMaxHealth(float percentage)
    {
        // Turn percentage to a decimal
        float percentageDecimal = percentage / 100;

        // Get percentage of the max health from health component
        float amountToAdd = m_healthComponent.GetMaxHealth() * percentageDecimal;

        // Remove percentage from the max health on health component
        m_healthComponent.IncreaseMaxHealth(amountToAdd);
    }
    public void IncreaseDamage(float percentage)
    {
        // Turn percentage to a decimal
        float percentageDecimal = percentage / 100;

        // Get percentage of the damage float
        float amountToAdd = m_attackDamage * percentageDecimal;

        // Remove percentage amount from the damage float
        m_attackDamage = m_attackDamage + amountToAdd;
    }

}