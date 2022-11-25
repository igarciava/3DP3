using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBehaviour : StateMachineBehaviour
{
    MarioPlayerController MarioPlayerController;
    public float StartPctTime = 0.3f;
    public float EndPctTime = 0.3f;
    public MarioPlayerController.TJumpType JumpType;
    bool JumpActive = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MarioPlayerController = animator.GetComponent<MarioPlayerController>();
        //MarioPlayerController.SetJumpActive(JumpType, false);
        JumpActive = false;
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!JumpActive && stateInfo.normalizedTime >= StartPctTime && stateInfo.normalizedTime <= EndPctTime)
        {
            JumpActive = true;
        }
        else if (JumpActive && stateInfo.normalizedTime > EndPctTime)
        {
            JumpActive = false;
        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MarioPlayerController.SetIsJumpEnabled(false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
