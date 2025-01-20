using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] private GameObject m_playerCam;
    [SerializeField] private GameObject m_player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_playerCam = GameObject.FindGameObjectWithTag("MainCamera"); // Find the main camera game object
        m_playerCam.transform.SetParent(m_player.transform); // Set the player as the cameras new parent
        m_playerCam.transform.localPosition = new Vector3(0, 0, -11); ; // Reset local position

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
