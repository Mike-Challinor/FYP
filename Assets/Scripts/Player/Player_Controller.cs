using UnityEngine;
using System.Collections;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] private GameObject m_playerCam;
    [SerializeField] private GameObject m_player;
    [SerializeField] private GameObject m_playerHUD;
    [SerializeField] private Player_HUD m_playerHUDScript;
    [SerializeField] private Player_Input m_playerInput;
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
        m_playerInput = GetComponent<Player_Input>();
        m_playerHUDScript.InitHUD(m_maxHealth);
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
            playerCamera.GetComponent<Camera>().fieldOfView = 71;
        }
        else
        {
            Debug.LogError("ERROR::PLAYERNETWORK::SPAWNPLAYERCAMERA:: Player camera is null");
        }
    }

    public void Interact()
    {
        InteractableObject interactableObject = m_interactableObject.GetComponent<InteractableObject>();

        // If the object is a door
        if (interactableObject.GetObjectType() == InteractableObject.InteractableType.Door)
        {
            // Check key count
            if (m_keyCount < 2)
            {
                // If the player does not have enough keys then inform the player that the door is locked and do not interact
                m_playerHUDScript.SetObtainedText("Door is locked");
                return;
            }

            m_playerHUDScript.SetObtainedText("Door unlocked");
        }

        m_interactableObject.GetComponent<InteractableObject>().Interact(this);
    }

    public void SetInteractionStatus(bool canInteract, GameObject interactObject)
    {
        m_interactableObject = interactObject;
        m_canInteract = canInteract;

        if (m_canInteract)
        {
            m_playerHUDScript.ShowInteractText(true);
        }

        else
        {
            m_playerHUDScript.ShowInteractText(false);
        }
    }

    public bool GetInteractionStatus()
    {
        return m_canInteract;
    }

    public void AddKey()
    {
        m_keyCount++;
        m_playerHUDScript.SetKeyText(m_keyCount);
        m_playerHUDScript.SetObtainedText("Key obtained");
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

        // Update UI
        m_playerHUDScript.SetObtainedText($"Attack Speed increased");
    }

    public void IncreaseMaxHealth(float percentage)
    {
        // Turn percentage to a decimal
        float percentageDecimal = percentage / 100;

        // Get percentage of the max health from health component
        float amountToAdd = m_healthComponent.GetMaxHealth() * percentageDecimal;

        // Remove percentage from the max health on health component
        m_healthComponent.IncreaseMaxHealth(amountToAdd);

        // Update UI
        m_playerHUDScript.SetObtainedText($"Health increased");
    }
    public void IncreaseDamage(float percentage)
    {
        // Turn percentage to a decimal
        float percentageDecimal = percentage / 100;

        // Get percentage of the damage float
        float amountToAdd = m_attackDamage * percentageDecimal;

        // Remove percentage amount from the damage float
        m_attackDamage = m_attackDamage + amountToAdd;

        // Update UI
        m_playerHUDScript.SetObtainedText($"Damage increased");
    }

    public void SetCanMove(bool canMove)
    {
        m_playerInput.SetCanMove(canMove);
    }

    public void SetHUDStatus(bool status)
    {
        if (status)
        {
            m_playerHUD.SetActive(true);
        }

        else
        {
            m_playerHUD.SetActive(false);
        }
    }

}