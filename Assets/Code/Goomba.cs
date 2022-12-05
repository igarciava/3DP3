using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Goomba : MonoBehaviour, IRestartGameElement
{
    CharacterController GoombaCC;
    NavMeshAgent TheNavMeshAgent;

    private void Start()
    {
        TheNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        GoombaCC = gameObject.GetComponent<CharacterController>();
        GameController.GetGameController().AddRestartGameElements(this);
    }
    public void Kill()
    {
        transform.localScale = new Vector3(1.0f, 0.2f, 1.0f);
        GoombaCC.enabled = false;
        TheNavMeshAgent.enabled = false;
        StartCoroutine(Hide());
    }
    IEnumerator Hide()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }

    void IRestartGameElement.RestartGame()
    {
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        gameObject.SetActive(true);
    }
}
