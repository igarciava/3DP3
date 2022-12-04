using System.Collections;
using UnityEngine;
public class MarioPlayerController : MonoBehaviour, IRestartGameElement
{
    public enum TPunchType
    {
        RIGHT_HAND = 0,
        LEFT_HAND,
        KICK
    }
    public enum TJumpType
    {
        NORMAL_JUMP = 0,
        SECOND_JUMP,
        THIRD_JUMP
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
    public float ComboJumpTime = 1.0f;
    float ComboJumpCurrentTime;
    TJumpType CurrentComboJump;
    bool IsJumpEnabled;
    float JumpTimer;

    [Header("Punch")]
    public float ComboPunchTime = 1.0f;
    float ComboPunchCurrentTime;
    TPunchType CurrentComboPunch;
    public Collider LeftHandCollider;
    public Collider RightHandCollider;
    public Collider KickCollider;
    bool IsPunchEnabled = false;

    [Header("Elevator")]
    public float ElevatorDotAngle = 0.95f;
    public Collider CurrentElevatorCollider = null;

    [Header("Bridge")]
    public float BridgeForce = 2.5f;

    [Header("CheckPoint")]
    public CheckPoint CurrentCheckPoint = null;

    [Header("Goomba")]
    public float MaxAngleToKillGoomba = 55.0f;
    public float KillerJumpSpeed = 1;

    [Header("Health")]
    bool IsDead = false;
    public HealthScript Health;

    [Header("Crouch")]
    bool IsCrouched = false;

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
        ComboJumpCurrentTime = -ComboJumpTime;
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
        if (PunchType == TPunchType.RIGHT_HAND)
        {
            RightHandCollider.gameObject.SetActive(Active);
        }
        if (PunchType == TPunchType.LEFT_HAND)
        {
            LeftHandCollider.gameObject.SetActive(Active);
        }
        if (PunchType == TPunchType.KICK)
        {
            KickCollider.gameObject.SetActive(Active);
        }

    }
    // Update is called once per frame
    void Update()
    {
        float l_SpeedParameter = 0.0f;

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
        if (Input.GetKeyDown(KeyCode.Space) && OnGround && CanJump())
        {
            if (IsCrouched == true)
            {
                VerticalSpeed = JumpSpeed;
                Animator.SetTrigger("LongJump");
            }
            else
            {
                if (MustRestartComboJump())
                {
                    SetComboJump(TJumpType.NORMAL_JUMP);
                }
                else
                    NextComboJump();
            }
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            IsCrouched = true;
        }
        else
        {
            IsCrouched = false;
            Debug.Log("iscrouched" + IsCrouched);
        }
        
        l_Movement.Normalize();

        float l_MovementSpeed = 0.0f;
        if (l_HasMovement)
        {
            Quaternion l_LookRotation = Quaternion.LookRotation(l_Movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, l_LookRotation, LerpRotationPct);

            l_SpeedParameter = 0.5f;
            l_MovementSpeed = WalkSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                l_SpeedParameter = 1.0f;
                l_MovementSpeed = RunSpeed;
            }
        }
        Animator.SetFloat("Speed", l_SpeedParameter);

        VerticalSpeed = VerticalSpeed + Physics.gravity.y * Time.deltaTime;
        l_Movement = l_Movement * l_MovementSpeed * Time.deltaTime;
        l_Movement.y = VerticalSpeed * Time.deltaTime;

        Animator.SetFloat("VSpeed", VerticalSpeed);

        CollisionFlags l_CollisionFlags = CC.Move(l_Movement);

        if ((l_CollisionFlags & CollisionFlags.Below) != 0 && VerticalSpeed < 0.0f)
        {
            VerticalSpeed = 0.0f;
            OnGround = true;
        }
        else
        {
            TimerOnGround();
        }

        if (OnGround == false)
        {
            if (VerticalSpeed < 0)
                Animator.SetBool("Falling", true);
        }
        else
        {
            Animator.SetBool("Falling", false);
        }

        if (Input.GetMouseButtonDown(0) && CanPunch())
        {
            if (MustRestartComboPunch())
                SetComboPunch(TPunchType.RIGHT_HAND);
            else
                NextComboPunch();
        }
        
        if(Input.GetKeyDown(KeyCode.M))
        {
            GetHit();
        }

        if(Health.CurrentHealth < 1)
        {
            IsDead = true;
        }
        else
            IsDead = false;

        if(IsDead)
        {
            Die();
        }
    }

    void TimerOnGround()
    {
        JumpTimer = JumpTimer + Time.deltaTime;

        if (JumpTimer >= 0.2f)
        {
            JumpTimer = 0.0f;
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
        if (hit.gameObject.tag == "Bridge")
        {
            hit.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * BridgeForce, hit.point);
        }
        else if (hit.gameObject.tag == "Goomba")
        {
            if (CanKillGoomba(hit.normal))
            {
                hit.gameObject.GetComponent<Goomba>().Kill();
                JumpOverEnemy();
            }
            else
                Debug.DrawRay(hit.point, hit.normal * 3.0f, Color.blue, 5.0f);
        }
    }

    //Goomba
    bool CanKillGoomba(Vector3 theNormal)
    {
        return Vector3.Dot(theNormal, Vector3.up) >= Mathf.Cos(MaxAngleToKillGoomba * Mathf.Deg2Rad);
    }
    void JumpOverEnemy()
    {
        VerticalSpeed = KillerJumpSpeed;
    }

    //Punch
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
        return (Time.time - ComboPunchCurrentTime) > ComboPunchTime;
    }
    void NextComboPunch()
    {
        if (CurrentComboPunch == TPunchType.RIGHT_HAND)
            SetComboPunch(TPunchType.LEFT_HAND);
        else if (CurrentComboPunch == TPunchType.LEFT_HAND)
            SetComboPunch(TPunchType.KICK);
        else if (CurrentComboPunch == TPunchType.KICK)
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
    //End Punch

    //Jump
    bool CanJump()
    {
        return !IsJumpEnabled;
    }
    public void SetIsJumpEnabled(bool TheJumpActive)
    {
        IsJumpEnabled = TheJumpActive;
    }
    bool MustRestartComboJump()
    {
        return (Time.time - ComboJumpCurrentTime) > ComboJumpTime;
    }
    void NextComboJump()
    {
        if (CurrentComboJump == TJumpType.NORMAL_JUMP)
            SetComboJump(TJumpType.SECOND_JUMP);
        else if (CurrentComboJump == TJumpType.SECOND_JUMP)
            SetComboJump(TJumpType.THIRD_JUMP);
        else if (CurrentComboJump == TJumpType.THIRD_JUMP)
            SetComboJump(TJumpType.NORMAL_JUMP);
    }
    void SetComboJump(TJumpType JumpType)
    {
        Debug.Log("jump " + JumpType);
        CurrentComboJump = JumpType;
        ComboJumpCurrentTime = Time.time;
        IsJumpEnabled = true;
        if (CurrentComboJump == TJumpType.NORMAL_JUMP)
        {
            VerticalSpeed = JumpSpeed;
            Animator.SetTrigger("JumpNormal");
        }
        else if (CurrentComboJump == TJumpType.SECOND_JUMP)
        {
            VerticalSpeed = JumpSpeed + 2;
            Animator.SetTrigger("SecondJump");
        }
        else if (CurrentComboJump == TJumpType.THIRD_JUMP)
        {
            VerticalSpeed = JumpSpeed + 4;
            Animator.SetTrigger("ThirdJump");
        }
        
        if(OnGround == true)
        {
            Animator.SetBool("IsGround", OnGround);
        }
        else if (OnGround == false)
        {
            Animator.SetBool("IsGround", OnGround);
        }
    }
    void LongJump()
    {
        Animator.SetTrigger("LongJump");
    }
    //End jump

    //Elevator/other things
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Elevator" && CanAttachToElevator(other))
        {
            AttachToElevator(other);
        }
        if(other.tag == "CheckPoint")
        {
            CurrentCheckPoint = other.GetComponent<CheckPoint>();
            other.GetComponentInChildren<ParticleSystem>().Play();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Elevator" && other == CurrentElevatorCollider)
            DetachElevator();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Elevator")
        {
            if (CurrentElevatorCollider == other && Vector3.Dot(other.transform.up, Vector3.up) < ElevatorDotAngle)
                DetachElevator();
            if (CanAttachToElevator(other))
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
    //End elevator

    public void GetHit()
    {
        Health.SubstractLife();
        Animator.SetBool("Hit", true);
        StartCoroutine(EndHit());
    }

    IEnumerator EndHit()
    {
        yield return new WaitForSeconds(0.1f);
        Animator.SetBool("Hit", false);
    }

    public void Die()
    {
        CC.enabled = false;
        Animator.SetBool("IsDead", IsDead);
    }

    public void RestartGame()
    {
        IsDead = false;
        CC.enabled = false;
        if (CurrentCheckPoint == null)
        {
            transform.position = StartPosition;
            transform.rotation = StartRotation;
        }
        else
        {
            transform.position = CurrentCheckPoint.SpawnPosition.position;
            transform.rotation = CurrentCheckPoint.SpawnPosition.rotation;
        }
        CC.enabled = true;
    }
}