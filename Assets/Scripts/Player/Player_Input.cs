using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Input : MonoBehaviour
{
    private PlayerInputActions m_playerActions;
    private Vector2 m_moveInput;
    private SpriteRenderer m_playerSprite;
    private Rigidbody2D m_RB;
    private Player_Controller m_playerController;
    private Vector3 m_firePointLocalPosition;

    [SerializeField] private float m_moveSpeed = 5f;
    [SerializeField] private bool m_isFacingRight = true;
    [SerializeField] private GameObject m_firePoint;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_RB = GetComponent<Rigidbody2D>();
        m_playerSprite = GetComponent<SpriteRenderer>();
        m_playerController = GetComponent<Player_Controller>();
        m_firePointLocalPosition = m_firePoint.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    private void OnEnable()
    {
        m_playerActions = new PlayerInputActions();

        m_playerActions.Enable();

        // Listen for movement input
        m_playerActions.Player.Move.performed += ctx => m_moveInput = ctx.ReadValue<Vector2>();
        // Stop movement on release
        m_playerActions.Player.Move.canceled += ctx => m_moveInput = Vector2.zero;

        // Initiate Attack when pressed
        m_playerActions.Player.Attack.performed += InitiateAttack;
        // Stop attacking on release
        m_playerActions.Player.Attack.canceled += EndAttack;
    }

    private void OnDisable()
    {
        // Unsub from player movement
        m_playerActions.Player.Move.performed -= ctx => m_moveInput = ctx.ReadValue<Vector2>();
        m_playerActions.Player.Move.canceled -= ctx => m_moveInput = Vector2.zero;

        // Unsub from player attack
        // Unsub from player attack
        m_playerActions.Player.Attack.performed -= InitiateAttack;
        m_playerActions.Player.Attack.canceled -= EndAttack;

        // Disable input
        m_playerActions.Disable();
    }

    private void MovePlayer()
    {
        // Cancel all movement if there is no input
        if (m_moveInput.x == 0 && m_moveInput.y == 0)
        {
            m_RB.linearVelocity = Vector2.zero;
        }

        m_RB.linearVelocity = m_moveInput * m_moveSpeed;
    }

    private void InitiateAttack(InputAction.CallbackContext ctx)
    {
        if (m_playerController.GetCanAttack()) // Initiate attack if player can attack
        {
            Debug.Log("Initiate Attack!");
            m_playerController.InitiateAttack();
        }        
    }

    private void EndAttack(InputAction.CallbackContext ctx)
    {
        Debug.Log("End Attack!");
        m_playerController.EndAttack();
    }

}
