using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
public class MarioPlayerController : MonoBehaviour
{
    public Animator Animator;
    public CharacterController CharacterController;
    
    public Camera Camera;
    public float LerpRotationPct = 0.3f;
    public float WalkSpeed = 2.5f;
    public float RunSpeed = 6.5f;
    public float Gravity = -5.0f;

    public float JumpSpeed = 10.0f;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        CharacterController = GetComponent<CharacterController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Gravity = JumpSpeed;
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
        l_Movement = l_Movement * l_MovementSpeed * Time.deltaTime;
        l_Movement.y = -1;
        if (Input.GetMouseButtonDown(0))
        {
            Animator.SetTrigger("Punch");
        }
        CharacterController.Move(l_Movement);
    }

    public void Die()
    {
        Debug.Log("det");
    }
}