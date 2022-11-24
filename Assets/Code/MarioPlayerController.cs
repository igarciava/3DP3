using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
public class MarioPlayerController : MonoBehaviour, IRestartGameElement
{
    public enum TPunchType
    {
        RIGHT_HAND = 0,
        LEFT_HAND,
        KICK
    }

    public Animator Animator;
    public CharacterController CC;
    
    public Camera Camera;
    public float LerpRotationPct = 0.3f;
    public float WalkSpeed = 2.5f;
    public float RunSpeed = 6.5f;
    
    [Header("Jump")]
    public float JumpSpeed = 10.0f;
    public float VerticalSpeed = 0.0f;
    bool OnGround = true;

    [Header("Punch")]
    public float ComboPunchTime = 2.5f;
    float ComboPunchCurrentTime;
    TPunchType CurrentComboPunch;
    public Collider LeftHandCollider;
    public Collider RightHandCollider;
    public Collider KickCollider;
    bool IsPunchEnabled = false;

    [Header("Elevator")]
    public float ElevatorDotAngle = 0.95f;
    public Collider CurrentElevatorCollider = null;

    [Header("Elevator")]
    public float BridgeForce = 2.5f;

    Vector3 StartPosition;
    Quaternion StartRotation;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        CC = GetComponent<CharacterController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        ComboPunchCurrentTime = -ComboPunchTime;
        LeftHandCollider.gameObject.SetActive(false);
        RightHandCollider.gameObject.SetActive(false);
        KickCollider.gameObject.SetActive(false);
        StartPosition = transform.position;
        StartRotation = transform.rotation;
        GameController.GetGameController().AddRestartGameElements(this);
        GameController.GetGameController().SetPlayer(this);
    }

    public void SetPunchActive(TPunchType PunchType, bool Active)
    {
        if(PunchType == TPunchType.RIGHT_HAND)
        {
            RightHandCollider.gameObject.SetActive(Active);
        }
        if(PunchType == TPunchType.LEFT_HAND)
        {
            LeftHandCollider.gameObject.SetActive(Active);
        }
        if(PunchType == TPunchType.KICK)
        {
            KickCollider.gameObject.SetActive(Active);
        }

    }
    // Update is called once per frame
    void Update()
    {
        float l_Speed = 0.0f;

        Vector3 l_ForwardCamera = Camera.transform.forward;
        Vector3 l_RightCamera = Camera.transform.right;
        l_ForwardCamera.y = 0.0f;
        l_RightCamera.y = 0.0f;
        l_ForwardCamera.Normalize();
        l_RightCamera.Normalize();
        bool l_HasMovement = false;

        Vector3 l_Movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            l_HasMovement = true; 
            l_Movement = l_ForwardCamera;
        }
        if (Input.GetKey(KeyCode.S))
        {
            l_HasMovement = true;
            l_Movement = -l_ForwardCamera;
        }
        if (Input.GetKey(KeyCode.A))
        {
            l_HasMovement = true;
            l_Movement -= l_RightCamera;
        }
        if (Input.GetKey(KeyCode.D))
        {
            l_HasMovement = true;
            l_Movement += l_RightCamera;
        }
        if (Input.GetKeyDown(KeyCode.Space) && OnGround)
        {
            VerticalSpeed = JumpSpeed;
        }
        l_Movement.Normalize();

        float l_MovementSpeed = 0.0f;
        if (l_HasMovement)
        {
            Quaternion l_LookRotation = Quaternion.LookRotation(l_Movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, l_LookRotation, LerpRotationPct);

            l_Speed = 0.5f;
            l_MovementSpeed = WalkSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                l_Speed = 1.0f;
                l_MovementSpeed = RunSpeed;
            }
        }
        Animator.SetFloat("Speed", l_Speed);
        VerticalSpeed = VerticalSpeed + Physics.gravity.y * Time.deltaTime;
        l_Movement.y = VerticalSpeed * Time.deltaTime;
        l_Movement = l_Movement * l_MovementSpeed * Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && CanPunch())
        {
            if (MustRestartComboPunch())
                SetComboPunch(TPunchType.RIGHT_HAND);
            else
                NextComboPunch();
        }

        CollisionFlags l_CollisionFlags = CC.Move(l_Movement);

        
        if ((l_CollisionFlags & CollisionFlags.Below) != 0 && VerticalSpeed < 0.0f)
        {
            VerticalSpeed = 0.0f;
            OnGround = true;
        }
        else
        {
            OnGround = false;
        }
    }

    private void LateUpdate()
    {
        if (CurrentElevatorCollider != null)
        {
            Vector3 l_EulerRotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0.0f, l_EulerRotation.y, 0.0f);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "Bridge")
        {
            hit.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * BridgeForce, hit.point);
        }
    }

    bool CanPunch()
    {
        return !IsPunchEnabled;
    }
    public void SetIsPunchEnabled(bool ThePunchActive)
    {
        IsPunchEnabled = ThePunchActive;
    }
    bool MustRestartComboPunch()
    {
        return (Time.time - ComboPunchCurrentTime)> ComboPunchTime;
    }
    void NextComboPunch()
    {
        if (CurrentComboPunch == TPunchType.RIGHT_HAND)
            SetComboPunch(TPunchType.LEFT_HAND);
        else if(CurrentComboPunch == TPunchType.LEFT_HAND)
            SetComboPunch(TPunchType.KICK);
        else if(CurrentComboPunch == TPunchType.KICK)
            SetComboPunch(TPunchType.RIGHT_HAND);
    }
    void SetComboPunch(TPunchType PunchType)
    {
        CurrentComboPunch = PunchType;
        ComboPunchCurrentTime = Time.time;
        IsPunchEnabled = true;
        if (CurrentComboPunch == TPunchType.RIGHT_HAND)
            Animator.SetTrigger("PunchRHand");
        else if (CurrentComboPunch == TPunchType.LEFT_HAND)
            Animator.SetTrigger("PunchLHand");
        else if (CurrentComboPunch == TPunchType.KICK)
            Animator.SetTrigger("PunchKick");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag =="Elevator" && CanAttachToElevator(other))
        {
            AttachToElevator(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Elevator" && other == CurrentElevatorCollider)
            DetachElevator();
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Elevator")
        {
            if (CurrentElevatorCollider == other && Vector3.Dot(other.transform.up, Vector3.up) < ElevatorDotAngle)
                DetachElevator();
            if(CanAttachToElevator(other))
            {
                AttachToElevator(other);
            }
        }
    }

    bool CanAttachToElevator(Collider other)
    {
        return CurrentElevatorCollider == null && Vector3.Dot(other.transform.up, Vector3.up) >= ElevatorDotAngle;
    }
    void AttachToElevator(Collider other)
    {
        transform.SetParent(other.transform);
        CurrentElevatorCollider = other;
    }
    void DetachElevator()
    {
        transform.SetParent(null);
        CurrentElevatorCollider = null;
    }

    public void Die()
    {
        Debug.Log("det");
    }

    public void RestartGame()
    {
        CC.enabled = false;
        transform.position = StartPosition;
        transform.rotation = StartRotation;
        CC.enabled = true;
    }
}