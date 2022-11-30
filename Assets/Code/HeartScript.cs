using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartScript : MonoBehaviour
{
    public GameObject Parent;
    public HealthScript HealthScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (HealthScript.CurrentHealth < 8)
            {
                HealthScript.Heal();
                Destroy(Parent);
            }
        }
    }
}
