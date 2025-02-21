using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Player_HUD : MonoBehaviour
{
    [SerializeField] private Slider m_healthSlider;
    [SerializeField] private TMP_Text m_currentHealthText;
    [SerializeField] private TMP_Text m_maxHealthText;
    [SerializeField] private TMP_Text m_keyText;
    [SerializeField] private TMP_Text m_obtainedText;
    [SerializeField] private GameObject m_interactText;
    [SerializeField] private float m_fadeDuration = 0.5f;

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

    public void SetKeyText(int keyCount)
    {
        // Set the key text
        string text = "x";
        m_keyText.text = text + keyCount.ToString();
    }

    public void SetObtainedText(string text)
    {
        // Set the text
        m_obtainedText.text = text;

        // Fade the text
        StartCoroutine(FadeText(m_obtainedText));
    }

    public void ShowInteractText(bool show)
    {
        m_interactText.SetActive(show);
    }

    private IEnumerator FadeText(TMP_Text textObject)
    {
        // Fade in text
        yield return StartCoroutine(FadeTextTimer(textObject, true));

        // Wait for 2 seconds
        yield return StartCoroutine(WaitForSeconds(0.25f));

        // Fade out text
        yield return StartCoroutine(FadeTextTimer(textObject, false));
    }

    // Timer for fading text
    private IEnumerator FadeTextTimer(TMP_Text textObject, bool fadeIn)
    {
        // Delay before starting fade in
        // yield return new WaitForSeconds(1);

        // Reset elapsed time variable
        float elapsedTime = 0f;

        // Loop through for fade duration
        while (elapsedTime < m_fadeDuration)
        {
            if (fadeIn)
            {
                // Lerp the alpha of the canvas group over the specified time
                textObject.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0f, 1f, elapsedTime / m_fadeDuration);
            }

            else
            {
                // Lerp the alpha of the canvas group over the specified time
                textObject.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, elapsedTime / m_fadeDuration);
            }

            // Increment the elapsed time using Delta time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure it's fully visible (or invisible) at the end of the coroutine
        if (fadeIn)
        {
            textObject.GetComponent<CanvasGroup>().alpha = 1f;
        }

        else
        {
            textObject.GetComponent<CanvasGroup>().alpha = 0f;
        }

    }
    private IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

}
