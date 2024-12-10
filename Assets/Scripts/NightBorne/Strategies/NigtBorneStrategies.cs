using UnityEngine;

public class MainStrategyForNightBorne : MainBossStrategy
{
   protected NightBorne nightBorne => 
        GameManager.instance.blackBoard.GetValue("NightBorne",out NightBorne _nightborne) ? _nightborne : null;
}


public class AttackStrategy : MainStrategyForNightBorne, IStrategy
{

    AnimatorStateInfo stateInfo;
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("AttackStrategy");
        nightBorne.rb.velocity = Vector2.zero;
        stateInfo = nightBorne.bossAnim.GetCurrentAnimatorStateInfo(0);
        nightBorne.attack = true;

        if (stateInfo.IsName("attack"))
        {
            if(stateInfo.normalizedTime >= 1)
            {
                nightBorne.attack = false;
                return Node.NodeStatus.SUCCESS;
            }
        }

        return Node.NodeStatus.RUNNING;
    }
}

public class InDeathStrategy : MainStrategyForNightBorne, IStrategy
{
    AnimatorStateInfo stateInfo;
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("DeathStrategy");

        stateInfo = nightBorne.bossAnim.GetCurrentAnimatorStateInfo(0);
        //nightBorne.rb.velocity = Vector2.zero;
        if (stateInfo.IsName("death"))
        {
            if(stateInfo.normalizedTime >= 1)
            {
                
                return Node.NodeStatus.SUCCESS;
            }
        }

        return Node.NodeStatus.RUNNING;
        
    }
}