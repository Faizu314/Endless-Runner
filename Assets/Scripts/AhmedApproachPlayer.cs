using UnityEngine;

public class AhmedApproachPlayer : StateMachineBehaviour
{
    [SerializeField] private Transform player;
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.Find("Player").transform;
        animator.gameObject.transform.rotation = Quaternion.Euler(0, -90f, 0);
        float distance = Vector3.SqrMagnitude(animator.gameObject.transform.position - player.position);
        animator.SetFloat("DistanceFromPlayer", distance);
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 current = animator.gameObject.transform.position;
        animator.gameObject.transform.position = Vector3.Lerp(current, player.position, Time.deltaTime * 0.1f);

        float distance = Vector3.SqrMagnitude(animator.gameObject.transform.position - player.position);
        animator.SetFloat("DistanceFromPlayer", distance);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
