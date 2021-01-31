using UnityEngine;

public class AhmedWaves : StateMachineBehaviour
{
    private EnemySpawner enemySpawner;
    private CustomFormations customFormations;
    private bool isLastFormation = false;
    private bool isDead = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemySpawner = GameObject.Find("Level Designer").GetComponent<EnemySpawner>();
        enemySpawner.SetBossCallBack(OnFormationDead);
        customFormations = animator.GetComponent<CustomFormations>();
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isDead)
        {
            animator.SetBool("WaveComplete", true);
            enemySpawner.Despawn(animator.gameObject);
        }
    }

    public void OnWaveEnded()
    {
        isLastFormation = true;
    }

    public void OnFormationDead()
    {
        if (isLastFormation)
            isDead = true;
        else
            customFormations.NextFormation(OnWaveEnded);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
