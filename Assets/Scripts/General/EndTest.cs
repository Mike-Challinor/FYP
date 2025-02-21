using UnityEngine;

public class EndTest : InteractableObject
{
    [SerializeField] GameManager m_gameManager;

    public override void Interact(Player_Controller controller)
    {
        m_gameManager.EndTest();
    }
}
