using UnityEngine;

public class StartTest : MonoBehaviour
{
    [SerializeField] GameManager m_gameManager;

    public void ButtonSelected()
    {
        m_gameManager.StartTest();
    }
}
