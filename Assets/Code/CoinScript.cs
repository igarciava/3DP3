using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinScript : MonoBehaviour
{
    public GameObject Parent;
    public HudAnimation Hud;

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameController.GetGameController().Coins += 1;
            Hud.MoveDown();
            Destroy(Parent);
        }
    }
}
