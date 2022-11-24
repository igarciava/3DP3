using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    static GameController m_GameController = null;
    MarioPlayerController Mario;
    List<IRestartGameElement> RestartGameElements = new List<IRestartGameElement>();

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public static GameController GetGameController()
    {
        if(m_GameController == null)
        {
            m_GameController = new GameObject("GameController").AddComponent<GameController>();
        }
        return m_GameController;
    }
    public static void DestroySingleton()
    {
        if(m_GameController != null)
        {
            GameObject.Destroy(m_GameController.gameObject);
        }
        m_GameController = null;
    }

    public void AddRestartGameElements(IRestartGameElement TheRestartGameElement)
    {
        RestartGameElements.Add(TheRestartGameElement);
    }
    public void SetPlayer(MarioPlayerController TheMario)
    {
        Mario = TheMario;
    }
    public void RestartGame()
    {
        foreach (IRestartGameElement l_restartGameElement in RestartGameElements)
        {
            l_restartGameElement.RestartGame();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            RestartGame();
    }
}
