using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchBehaviour : StateMachineBehaviour
{
    MarioPlayerController MarioPlayerController;
    public float StartPctTime = 0.3f;
    public float EndPctTime = 0.3f;
    public MarioPlayerController.TPunchType PunchType;
    bool PunchActive = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MarioPlayerController = animator.GetComponent<MarioPlayerController>();
        MarioPlayerController.SetPunchActive(PunchType, false);
        PunchActive = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!PunchActive && stateInfo.normalizedTime>=StartPctTime && stateInfo.normalizedTime<=EndPctTime)
        {
            MarioPlayerController.SetPunchActive(PunchType, true);
            PunchActive = true;
        }
        else if(PunchActive && stateInfo.normalizedTime>EndPctTime)
        {
            MarioPlayerController.SetPunchActive(PunchType, false);
            PunchActive = false;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MarioPlayerController.SetPunchActive(PunchType, false);
        MarioPlayerController.SetIsPunchEnabled(false);
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
