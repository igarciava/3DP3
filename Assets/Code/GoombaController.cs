using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoombaController : MonoBehaviour
{
    public enum TState
    {
      
        PATROL = 0,
        ALERT,
        CHASE,
        ATTACK,
        HIT,
        DIE
    }

    public Animator Animator;

    public List<Transform> m_PatrolTargets;
    public TState m_State;
    NavMeshAgent m_NavMeshAgent;
    int m_CurrentPatrolTargetID = 0;

 
    public float m_HearingDistance;
    public float m_VisualConeAngle = 60.0f;
    public float m_SightDistance = 8.0f;
    public LayerMask m_SightLayerMask;
    public float m_EyesHeight = 1.8f;
    public float m_EyesPlayerHeight = 1.8f;
    public float m_rotationSpeed = 60.0f;
    public float m_Speed = 10.0f;

    ParticleSystem DyingParticles;

    Vector3 m_PlayerPosition;
    Vector3 m_DistanceBetween;

    float m_GoombaSpeed = 0.5f;


    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
    }
    private void Start()
    {
        SetPatrolState();
    }
    private void Update()
    {
        switch (m_State)
        {
            
            case TState.PATROL:
                UpdatePatrolState();
                break;
            case TState.CHASE:
                UpdateChaseState();
                break;
           // case TState.DIE:
             //   UpdateDieState();
               // break;
        }

        
        Animator.SetFloat("GoombaSpeed", m_GoombaSpeed);

        Vector3 l_PlayerPosition = GameController.GetGameController().GetPlayer().transform.position;
        Vector3 l_EyesPosition = transform.position + Vector3.up * m_EyesHeight;
        Vector3 l_PlayerEyesPosition = l_PlayerPosition + Vector3.up * m_EyesPlayerHeight;
        Vector3 l_DistanceBetween = m_NavMeshAgent.transform.position - l_PlayerPosition;
        m_PlayerPosition = l_PlayerPosition;
        m_DistanceBetween = l_DistanceBetween;
    }

    void SetPatrolState()
    {
        m_State = TState.PATROL;
        m_NavMeshAgent.destination = m_PatrolTargets[m_CurrentPatrolTargetID].position;
    }
    void UpdatePatrolState()
    {
        if (PatrolTargetPositionArrived())
            MoveToNextTargetPosition();
        if (SeesPlayer())
        {
            Animator.SetBool("Sees", true);
            SetChaseState();
        }
            
    }

    bool PatrolTargetPositionArrived()
    {
        return !m_NavMeshAgent.hasPath && !m_NavMeshAgent.pathPending && m_NavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete;
    }
    void MoveToNextTargetPosition()
    {
        ++m_CurrentPatrolTargetID;
        if (m_CurrentPatrolTargetID >= m_PatrolTargets.Count)
            m_CurrentPatrolTargetID = 0;
        m_NavMeshAgent.destination = m_PatrolTargets[m_CurrentPatrolTargetID].position;
    }

    bool SeesPlayer()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().GetPlayer().transform.position;
        Vector3 l_DirectionPlayerXZ = l_PlayerPosition - transform.position;
        l_DirectionPlayerXZ.Normalize();
        Vector3 l_ForwardXZ = transform.forward;
        l_ForwardXZ.Normalize();

        Vector3 l_EyesPosition = transform.position + Vector3.up * m_EyesHeight;
        Vector3 l_PlayerEyesPosition = l_PlayerPosition + Vector3.up * m_EyesPlayerHeight;
        Vector3 l_Direction = l_PlayerPosition - l_EyesPosition;
        float l_Lenght = l_Direction.magnitude;
        l_Direction /= l_Lenght;

        Ray l_Ray = new Ray(l_PlayerEyesPosition, l_Direction);

        return Vector3.Distance(l_PlayerPosition, transform.position) < m_SightDistance && Vector3.Dot(l_ForwardXZ, l_DirectionPlayerXZ) > Mathf.Cos(m_VisualConeAngle * Mathf.Deg2Rad / 2.0f) &&
            !Physics.Raycast(l_Ray, l_Lenght, m_SightLayerMask.value);
    }

    void SetChaseState()
    {
        m_State = TState.CHASE;
        m_NavMeshAgent.transform.LookAt(m_PlayerPosition);
    }
    void UpdateChaseState()
    {      
        m_NavMeshAgent.isStopped = false;
        m_NavMeshAgent.destination = m_PlayerPosition;
        m_Speed = 1.0f;
    }
}
