using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetBool : StateMachineBehaviour
{
    public string isInteractingBool;    // the bool parameter on the animator
    public bool isInteractingStatus;    // Set to false by default, in order to reset the bool parameter to false when the control reaches "Empty" State

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    // This method is called when the landing animation is over, when Player is allowed to move or rotate
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(isInteractingBool, isInteractingStatus);
    }
}
