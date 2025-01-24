using UnityEngine;

public class Health_Component : MonoBehaviour
{
    [SerializeField] private float m_maxHealth = 150.0f;
    [SerializeField] private float m_currentHealth;

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
    }

    public float GetHealth()
    {
        return m_currentHealth;
    }

    // Function for despawning the enemy
    private void Death()
    {
        Destroy(gameObject);
    }


}
