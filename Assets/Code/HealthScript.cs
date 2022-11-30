using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{
    Image Health;
    float MaxHealth = 8.0f;
    public float CurrentHealth = 8.0f;
    float MaxHealthCanGet = 8.0f;
    float MinHealthCanGet = 0.0f;
    HudAnimation HUD;

    [Header ("Colors")]
    public Color Blue;
    public Color Green;
    public Color Yellow;
    public Color Red;
    // Start is called before the first frame update
    void Start()
    {
        HUD = gameObject.GetComponentInParent<HudAnimation>();
        Health = gameObject.GetComponent<Image>();
        //Alpha Colors
        Blue.a = 1;
        Green.a = 1;
        Yellow.a = 1;
        Red.a = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentHealth > MaxHealthCanGet)
        {
            CurrentHealth = 8.0f;
        }
        else if(CurrentHealth < MinHealthCanGet)
        {
            CurrentHealth = 0.0f;
        }

        Health.fillAmount = CurrentHealth / MaxHealth;

        Color();
    }

    public void SubstractLife()
    {
        CurrentHealth -= 1.0f;
        HUD.MoveDown();
    }

    public void Heal()
    {
        CurrentHealth += 1.0f;
        HUD.MoveDown();
    }

    void Color()
    {
        if (CurrentHealth == 1 || CurrentHealth == 2)
            Health.color = Red;
        else if (CurrentHealth == 3 || CurrentHealth == 4)
            Health.color = Yellow;
        else if (CurrentHealth == 5 || CurrentHealth == 6)
            Health.color = Green;
        else if (CurrentHealth == 7 || CurrentHealth == 8)
            Health.color = Blue;
    }
}
