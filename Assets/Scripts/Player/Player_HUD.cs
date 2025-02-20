using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player_HUD : MonoBehaviour
{
    [SerializeField] private Slider m_healthSlider;
    [SerializeField] private TMP_Text m_currentHealthText;
    [SerializeField] private TMP_Text m_maxHealthText;
    [SerializeField] private GameObject m_interactText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        
    }

    public void InitHUD(float maxHealth)
    {
        // Set the max health on slider
        SetMaxHealthOnSlider(maxHealth);

        // Set the current health on the slider
        SetCurrentHealthOnSlider(maxHealth);

    }

    public void SetCurrentHealthOnSlider(float currentHealth)
    {
        // Set the current health on the slider
        m_healthSlider.value = currentHealth;
        m_currentHealthText.text = currentHealth.ToString();
    }

    public void SetMaxHealthOnSlider(float currentHealth)
    {
        // Set the max health on slider
        m_healthSlider.maxValue = currentHealth;
        m_maxHealthText.text = currentHealth.ToString();
    }

    public void ShowInteractText(bool show)
    {
        m_interactText.SetActive(show);
    }

}
