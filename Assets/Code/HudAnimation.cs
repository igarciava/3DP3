using UnityEngine;
using UnityEngine.UI;

public class HudAnimation : MonoBehaviour
{
    Animator AnimatorController;
    public bool ActivateHud;

    private bool Showing;
    private float SecsWithoutChange;

    [Header("Coins")]
    public Text CoinText;

    private void Start()
    {
        AnimatorController = gameObject.GetComponent<Animator>();
        SecsWithoutChange = 0.0f;
    }
    private void Update()
    {

        if (Showing)
        {

            SecsWithoutChange += Time.deltaTime;

            if (SecsWithoutChange > 2.0f)
            {
                HideHUD();
            }
        }
        ShowCoins();
    }

    void MoveUp()
    {
        if (Showing)
            AnimatorController.SetBool("SomethingHasHappened", false);
    }
    public void MoveDown()
    {
        SecsWithoutChange = 0.0f;
        if (!Showing)
        {
            AnimatorController.SetBool("SomethingHasHappened", true);
            Showing = true;
        }
    }

    void HideHUD()
    {

        MoveUp();
        Showing = false;
    }
    void ShowCoins()
    {
        CoinText.text = "" + GameController.GetGameController().Coins;
    }
}
