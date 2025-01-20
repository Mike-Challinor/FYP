using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Input : MonoBehaviour
{
    private PlayerInputActions m_playerActions;
    private Vector2 m_moveInput;
    private SpriteRenderer m_playerSprite;
    private Rigidbody2D m_RB;

    [SerializeField] private float m_moveSpeed = 5f;
    [SerializeField] private bool m_isFacingRight = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_RB = GetComponent<Rigidbody2D>();
        m_playerSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    protected virtual void OnEnable()
    {
        m_playerActions = new PlayerInputActions();

        m_playerActions.Enable();

        // Listen for movement input
        m_playerActions.Player.Move.performed += ctx => m_moveInput = ctx.ReadValue<Vector2>();
        // Stop movement on release
        m_playerActions.Player.Move.canceled += ctx => m_moveInput = Vector2.zero;

    }

    protected virtual void OnDisable()
    {
        m_playerActions.Player.Move.performed -= ctx => m_moveInput = ctx.ReadValue<Vector2>();
        m_playerActions.Player.Move.canceled -= ctx => m_moveInput = Vector2.zero;
        m_playerActions.Disable();
    }

    private void MovePlayer()
    {
        //Check to see if moving left or right
        if ((m_moveInput.x < 0 && m_isFacingRight) || (m_moveInput.x > 0 && !m_isFacingRight))
        {
            FlipSpriteRpc();
        }

        m_RB.linearVelocity = m_moveInput * m_moveSpeed;


    }

    protected virtual void FlipSpriteRpc()
    {
        m_isFacingRight = !m_isFacingRight;
        m_playerSprite.flipX = !m_playerSprite.flipX;
    }

}
