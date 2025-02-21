using UnityEngine;

public class RewardChest : InteractableObject
{
    private float m_percentage = 8f;

    [SerializeField] private Sprite m_openChestSprite;
    [SerializeField] private bool m_isKey = false;
    [SerializeField] private SpriteRenderer m_SR;

    private Upgrade m_selectedUpgrade;

    private enum Upgrade
    {
        HealthUpgrade,
        DamageUpgrade,
        ShootSpeedUpgrade
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set the type of the object
        m_type = InteractableType.Chest;

        if (!m_isKey)
        {
            // Select a random Upgrade value directly
            Upgrade[] upgrades = (Upgrade[])System.Enum.GetValues(typeof(Upgrade));
            m_selectedUpgrade = upgrades[Random.Range(0, upgrades.Length)];
        }
    }

    public override void Interact(Player_Controller controller)
    {
        GetReward(controller);
    }

    private void GetReward(Player_Controller controller)
    {
        if (base.m_isOpen) return;
        base.m_isOpen = true;

        // If the interaction status is true then set it to false
        if (controller.GetInteractionStatus())
        {
            controller.SetInteractionStatus(false, null);
        }

        // Open the chest (change the sprite)
        m_SR.sprite = m_openChestSprite;

        if (m_isKey)
        {
            controller.AddKey();
            return;
        }

        // Directly use the enum value in the switch
        switch (m_selectedUpgrade)
        {
            case Upgrade.HealthUpgrade:
                IncreaseHealth(controller);
                break;
            case Upgrade.DamageUpgrade:
                IncreaseDamage(controller);
                break;
            case Upgrade.ShootSpeedUpgrade:
                IncreaseShootSpeed(controller);
                break;
            default:
                Debug.LogWarning("Unknown upgrade selected!");
                break;
        }
    }

    private void IncreaseHealth(Player_Controller controller)
    {
        Debug.Log("Increased Health by " + m_percentage + "%!");

        // Logic to increase player's health
        controller.IncreaseMaxHealth(m_percentage);
    }

    private void IncreaseDamage(Player_Controller controller)
    {
        Debug.Log("Increased Damage by " + m_percentage + "%!");
        // Logic to increase player's damage
        controller.IncreaseDamage(m_percentage);
    }

    private void IncreaseShootSpeed(Player_Controller controller)
    {
        Debug.Log("Increased Shoot Speed by " + m_percentage + "%!");
        // Logic to increase player's shoot speed
        controller.DecreaseAttackTimer(m_percentage);
    }

    public void SetIsKey(bool isKey)
    {
        m_isKey = isKey;
    }
}
