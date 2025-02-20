using UnityEngine;

public class Health_Component : MonoBehaviour
{
    [SerializeField] private float m_maxHealth = 150.0f;
    [SerializeField] private float m_currentHealth;
    [SerializeField] private Player_HUD m_playerHUD;

    public void InitHealth(float maxHealth)
    {
        m_maxHealth = maxHealth;
        m_currentHealth = m_maxHealth;
    }

    public void AddHealth(float healthToAdd)
    {
        if (m_currentHealth + healthToAdd > m_maxHealth) // Check to make sure health does not exceed max health
        {
            m_currentHealth = m_maxHealth;
        }

        else
        {
            m_currentHealth = m_currentHealth + healthToAdd;
        }
    }

    public void RemoveHealth(float healthToRemove)
    {
        if (m_currentHealth - healthToRemove < 0) // Check to make sure health does not fall below 0
        {
            m_currentHealth = 0;
            Death();
        }

        else
        {
            m_currentHealth = m_currentHealth - healthToRemove;
        }

        m_playerHUD.SetCurrentHealthOnSlider(m_currentHealth);
    }

    public float GetHealth()
    {
        return m_currentHealth;
    }
    public float GetMaxHealth()
    {
        return m_maxHealth;
    }

    public void IncreaseMaxHealth(float healthToAdd)
    {
        m_maxHealth = m_maxHealth + healthToAdd;

        m_playerHUD.SetMaxHealthOnSlider(m_maxHealth);
    }

    // Function for despawning the enemy
    private void Death()
    {
        Destroy(gameObject);
    }


}
