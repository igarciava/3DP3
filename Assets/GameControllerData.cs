using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//[CreateAssetMenu(fileName = "GameControllerData", menuName = "TecnoCampus/Create GameControllerData", order = 1)]
public class GameControllerData : ScriptableObject
{
    [Header("Coins")]
    public Text CoinText;
    public int Coins;


    [Header("Health")]
    public HealthScript HealthScript;
}
