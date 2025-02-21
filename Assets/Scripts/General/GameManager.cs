using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] DungeonGenerator m_dungeonGenerator;
    [SerializeField] GameObject m_startGameCanvas;
    [SerializeField] GameObject m_endGameCanvas;
    [SerializeField] Player_Controller m_playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadTest());
    }

    private IEnumerator LoadTest()
    {
        yield return new WaitForSeconds(0.3f);
        m_dungeonGenerator.StartGenerateDungeonCoroutine();
    }

    public void StartTest()
    {
        m_startGameCanvas.SetActive(false);
        m_playerController.SetHUDStatus(true);
        m_playerController.SetCanMove(true);
    }

    public void EndTest()
    {
        m_playerController.SetHUDStatus(false);
        m_playerController.SetCanMove(false);
        m_endGameCanvas.SetActive(true);
    }
}
