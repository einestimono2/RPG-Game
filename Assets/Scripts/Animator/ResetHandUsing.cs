using UnityEngine;

public class ResetHandUsing : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
       PlayerManager playerManager = animator.GetComponent<PlayerManager>();

       playerManager.isUsingLeftHand = false;
       playerManager.isUsingRightHand = false;
    }
}
