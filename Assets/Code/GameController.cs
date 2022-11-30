using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    static GameController m_GameController = null;
    MarioPlayerController Mario;
    List<IRestartGameElement> RestartGameElements = new List<IRestartGameElement>();

    [Header ("Coins")]
    public Text CoinText;
    public int Coins;


    [Header ("Health")]
    public HealthScript HealthScript;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        Coins = 0;
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
    public MarioPlayerController GetPlayer()
    {
        return Mario;
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

        //Update Coins

        ShowCoins();
        MaxCoinsReached();

        //End Update Coins
    }

    //Coins

    void ShowCoins()
    {
        CoinText.text = "" + Coins;
    }

    void MaxCoinsReached()
    {
        if (Coins > 100)
        {
            Coins = 0;
            HealthScript.Heal();
        }
    }

    //End Coins
}
