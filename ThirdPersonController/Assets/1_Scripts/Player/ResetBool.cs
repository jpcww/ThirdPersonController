using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class to reset the bool parameter in Animator
public class ResetBool : StateMachineBehaviour
{
    public string isInteractingBool;    // the bool parameter on the animator
    public bool isInteractingStatus;    // Set to false by default, in order to reset the bool parameter to false when the control reaches "Empty"

    public string isUsingRootMotionBool;    // the bool parameter on the animator
    public bool isUsingRootMotionStatus;    // Set to false by default, in order to reset the bool parameter to false when the control reaches "Empty"

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)   // Called to reset the bool parameters when the control enters "Empty" state
    {
        animator.SetBool(isInteractingBool, isInteractingStatus);
        animator.SetBool(isUsingRootMotionBool, isUsingRootMotionStatus);
    }
}
