﻿using UnityEngine;

public class GameController : MonoBehaviour
{
    static GameController m_GameController = null;
    MarioPlayerController m_Player;
    float m_PlayerLife;
    float m_PlayerShield;
    float m_DroneDamage;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    public static GameController GetGameController()
    {
        if(m_GameController == null)
        {
            m_GameController = new GameObject("GameController").AddComponent<GameController>();
            GameControllerData l_GameControllerData = Resources.Load <GameControllerData>("GameControllerData");
            m_GameController.m_PlayerLife = l_GameControllerData.m_lifes;
            Debug.Log("Data loaded with life" + m_GameController.m_PlayerLife);
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

    public void SetPLayerLife(float PlayerLife)
    {
        m_PlayerLife = PlayerLife;
    }
    public float GetPlayerLife()
    {
        return m_PlayerLife;
    }
    public void SetPLayerShield(float PlayerShield)
    {
        m_PlayerShield = PlayerShield;
    }
    public float GetPlayerShield()
    {
        return m_PlayerShield;
    }

    public void SetDroneDamage(float DroneDamage)
    {
        m_DroneDamage = DroneDamage;
    }
    public float GetDroneDamage()
    {
        return m_DroneDamage;
    }
    public MarioPlayerController GetPlayer()
    {
        return m_Player;
    }
    public void SetPlayer(MarioPlayerController Player)
    {
        m_Player = Player;
    }
    public void RestartGame()
    {
        m_Player.RestartGame();
    }
}
