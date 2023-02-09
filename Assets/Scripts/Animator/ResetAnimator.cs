using UnityEngine;

public class ResetAnimator : StateMachineBehaviour
{
    public string target;
    public bool status;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        animator.SetBool(target, status);
    }
}
