using UnityEngine;

public class InteractableObject : MonoBehaviour
{

    protected bool m_isOpen = false;

    public enum InteractableType
    {
        Chest,
        Door        
    }

    protected InteractableType m_type;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !GetIsOpen())
        {
            Player_Controller controller = collision.GetComponent<Player_Controller>();
            controller.SetInteractionStatus(true, gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player_Controller controller = collision.GetComponent<Player_Controller>();

            if (controller.GetInteractionStatus())
            {
                controller.SetInteractionStatus(false, null);
            }

        }
    }

    public virtual void Interact(Player_Controller controller)
    {
        // Base function for interacting with objects
    }

    public InteractableType GetObjectType()
    {
        return m_type;
    }

    protected bool GetIsOpen()
    {
        return m_isOpen;
    }
}
