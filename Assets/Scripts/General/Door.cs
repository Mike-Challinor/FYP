using UnityEngine;

public class Door : InteractableObject
{
    [SerializeField] private SpriteRenderer m_SR;
    [SerializeField] private Sprite m_openDoorSprite;
    [SerializeField] private GameObject m_openCollider;
    [SerializeField] private GameObject m_closedCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Set the type of the object
        m_type = InteractableType.Door;
    }

    public override void Interact(Player_Controller controller)
    {
        OpenDoor(controller);
    }

    private void OpenDoor(Player_Controller controller)
    {
        if (base.GetIsOpen()) return;
        base.m_isOpen = true;

        // If the interaction status is true then set it to false
        if (controller.GetInteractionStatus())
        {
            controller.SetInteractionStatus(false, null);
        }

        // Open the chest (change the sprite)
        m_SR.sprite = m_openDoorSprite;

        // Disable the closed door collision
        m_closedCollider.SetActive(false);

        // Enable the open door collision
        m_openCollider.SetActive(true);

    }
    
}
