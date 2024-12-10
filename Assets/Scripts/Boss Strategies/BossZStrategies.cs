using System.Collections;
using UnityEngine;

public class MainStrategyForBossZ : MainBossStrategy
{
    protected BossZ bossZ =>
        GameManager.instance.blackBoard.GetValue("BossZ", out BossZ _bossZ) ? _bossZ : null;
}

public class HandleMovementStrategy : MainStrategyForBossZ, IStrategy
{
    public enum State
    {
        moveToChoosedPos,
        moveToPlayerOffset
    }

    State state;
    float offSet = 1.5f;
    float rightOffsetDistance =>
        Vector2.Distance(new Vector2(playerController.offset.transform.position.x, 0),
            new Vector2(playerController.transform.position.x,0));
    Vector2 playerOffsetPosition => new Vector2(playerController.offset.transform.position.x, playerController.offset.transform.position.y + offSet);
    
    Vector2 choosedPosition;
    bool isPositionChoosed = false;
    float speed;
    float distance = 0.1f;

    public HandleMovementStrategy(State state,Vector2 moveTo,float speed)
    {
        this.state = state;
        this.choosedPosition = moveTo;
        this.speed = speed;
    }
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("HandleMovementStrategy");
        
        if(state == State.moveToPlayerOffset)
        {
            bossZ.meleeSequence = true;
            choosedPosition = playerOffsetPosition;
        }

        if(state == State.moveToChoosedPos)
        {
            bossZ.meleeSequence = false;
        }
        

        bossZ.transform.position = Vector2.MoveTowards(bossZ.transform.position, choosedPosition, Time.deltaTime * speed);
        if (Vector2.Distance(bossZ.transform.position, choosedPosition) <= distance)
        {
            isPositionChoosed = false;
            return Node.NodeStatus.SUCCESS;
        }

        return Node.NodeStatus.RUNNING;
    }

}

public class MeleeAttackStrategy : MainStrategyForBossZ, IStrategy
{
    float distance = 1f;
    float forcePower = 2f;
    bool getAway = false;
    
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("MeleeAttackStrategy");
        AnimatorStateInfo stateInfo = bossZ.bossAnim.GetCurrentAnimatorStateInfo(0);
        bossZ.canMeleeAttack = true;

        if (stateInfo.IsName("melee_attack"))
        {
            if (stateInfo.normalizedTime >= 1)
            {
                bossZ.canMeleeAttack = false;
                getAway = false;
                blockCoroutine = false;
                return Node.NodeStatus.SUCCESS;
            }
            if (Vector2.Distance(new Vector2(bossZ.transform.position.x,0), new Vector2(playerController.transform.position.x,0)) < distance && !getAway && !playerController.isInDash)
            {
                if (!blockCoroutine)
                {
                    blockCoroutine = true;
                    bossZ.StartCoroutine(Timer());
                }
                bossZ.rb.AddForce(new Vector2(bossZ.direction.x < 0 ? forcePower : -1 * forcePower, 0), ForceMode2D.Impulse);
               
                return Node.NodeStatus.RUNNING;
            }

        }
        return Node.NodeStatus.RUNNING;
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.2f);
        bossZ.rb.velocity = Vector2.zero;
        getAway = true;
        blockCoroutine = false;

    }

}

public class DarkSpellStrategy : MainStrategyForBossZ, IStrategy
{
    Spell darkSpell;
    float playerPosYOffset = 1f;
    float playerOffsetPosYOffset = 2f;
    Vector2 playerPosition => playerController.transform.position;
    Vector2 playerOffsetPosition => playerController.offset.transform.position;
    public DarkSpellStrategy(Spell darkSpell)
    {
        this.darkSpell = darkSpell;
    }

    public Node.NodeStatus Evaluate()
    {
        Debug.Log("DarkSpellStrategy");
        Spell instantiatedDarkSpell = 
            GameObject.Instantiate(darkSpell,
            new Vector2(playerController.rb.velocity != Vector2.zero ? playerOffsetPosition.x : playerPosition.x,
            playerController.rb.velocity != Vector2.zero ? playerOffsetPosition.y + playerOffsetPosYOffset : playerPosition.y + playerPosYOffset),Quaternion.identity);

        return Node.NodeStatus.SUCCESS;
    }
}

public class CastSpellStrategy : MainStrategyForBossZ, IStrategy
{
    AnimatorStateInfo stateInfo;
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("CastSpellStrategy");
        stateInfo = bossZ.bossAnim.GetCurrentAnimatorStateInfo(0);
        if (bossZ.inPhaseTwo)
        {
            bossZ.phaseTwoCastSpell = true;
            if (stateInfo.IsName("phaseTwoCastSpell"))
            {
                if (stateInfo.normalizedTime >= 1)
                {
                    bossZ.phaseTwoCastSpell = false;
                    return Node.NodeStatus.SUCCESS;
                }
            }
        }
        else
        {
            bossZ.phaseOneCastSpell = true;
            if (stateInfo.IsName("phaseOne_cast_spell"))
            {
                if (stateInfo.normalizedTime >= 1)
                {
                    bossZ.phaseOneCastSpell = false;
                    return Node.NodeStatus.SUCCESS;
                }
            }
        }
        
        return Node.NodeStatus.RUNNING;
    }
}

public class DemonSpellStrategy : MainStrategyForBossZ, IStrategy
{
    int spellCount = 8;
    Spell instantiatedSpell;
    float increaseOffsetAmount = 2;
    float secondsToWait = 0.3f;

    Vector2 closestWayPoint => 
        (Vector2.Distance(bossZ.demonSpellWayPointOne.transform.position, playerController.transform.position) <
          Vector2.Distance(bossZ.demonSpellWayPointTwo.transform.position, playerController.transform.position)) ? bossZ.demonSpellWayPointOne.transform.position
        :bossZ.demonSpellWayPointTwo.transform.position;
    Vector2 capturedClosestWayPoint;
    public DemonSpellStrategy(Spell instantiatedSpell)
    {
        this.instantiatedSpell = instantiatedSpell;

    }

    public Node.NodeStatus Evaluate()
    {
        Debug.Log("DemonSpellStrategy");
        capturedClosestWayPoint = closestWayPoint;
        SpellLogic();
        return Node.NodeStatus.SUCCESS;
    }

    private void SpellLogic()
    {
        bossZ.StartCoroutine(Cast());
    }

    private IEnumerator Cast()
    {
        Vector2 spellPosition = closestWayPoint;
        bossZ.demonSpellInProgress = true;
        for (int i = 0; i < 10; i++)
        {
            Spell demonFire = GameObject.Instantiate(instantiatedSpell);
            demonFire.transform.position = spellPosition;
            spellPosition = new Vector2(capturedClosestWayPoint.x < bossZ.transform.position.x ? 
                spellPosition.x += increaseOffsetAmount : spellPosition.x -= increaseOffsetAmount, spellPosition.y);
            yield return new WaitForSeconds(secondsToWait);

        }
        bossZ.demonSpellInProgress = false;
    }
}

public class DragonSpellStrategy : MainStrategyForBossZ, IStrategy
{

    float amount = 1;
    Spell blueDragon;
    Spell redDragon;
    float offset = 1f;
    float distanceToLeft => Vector2.Distance(playerController.transform.position, bossZ.dragonSpellWayPointLeft.transform.position);
    float distanceToRight => Vector2.Distance(playerController.transform.position, bossZ.dragonSpellWayPointRight.transform.position);

    Vector2 closestWayPoint => distanceToLeft < distanceToRight ? bossZ.dragonSpellWayPointLeft.transform.position : bossZ.dragonSpellWayPointRight.transform.position;
    Vector2 capturedPosition;

    public DragonSpellStrategy(Spell blueDragon, Spell redDragon)
    {
        this.blueDragon = blueDragon;
        this.redDragon = redDragon;
    }

    public Node.NodeStatus Evaluate()
    {
       
        capturedPosition = closestWayPoint; 
        bossZ.StartCoroutine(CastSpell());
        bossZ.dragonSpellInProgress = true;
        return Node.NodeStatus.SUCCESS;
    }

    private IEnumerator CastSpell()
    {
        for(int i = 0; i < amount; i++)
        {
            Spell instantiatedSpell;
            if (playerController.transform.position.x < bossZ.centerPoint.transform.position.x)
            {
                instantiatedSpell = GameObject.Instantiate(redDragon);

            }
            else
            {
                instantiatedSpell = GameObject.Instantiate(blueDragon);
            }

            instantiatedSpell.transform.position = capturedPosition;
            capturedPosition = new Vector2(capturedPosition.x, capturedPosition.y+=offset);

            yield return new WaitForSeconds(0.3f);

        }
        bossZ.dragonSpellInProgress = false;
       
    }
}

public class FireLightningSpellStrategy : MainStrategyForBossZ, IStrategy
{
    Spell fireLightning;
    Transform lightningYPos;
    AnimationClip lightningClip;

    public FireLightningSpellStrategy(Spell fireLightning,Transform lightningYPos)
    {
        this.fireLightning = fireLightning;
        this.lightningYPos = lightningYPos;
        foreach(var clip in fireLightning.GetComponent<Animator>().runtimeAnimatorController.animationClips)
        {
            if(clip.name == "spellTwoAnim")
            {
                lightningClip = clip;
            }
        }
    }

    public Node.NodeStatus Evaluate()
    {
        CastSpell();
        bossZ.StartCoroutine(Wait());
        return Node.NodeStatus.SUCCESS;
    }

    private void CastSpell()
    {
        bossZ.ligtningSpellInProgress = true;
        GameObject.Instantiate(fireLightning,
            new Vector2(playerController.transform.position.x,lightningYPos.transform.position.y) , Quaternion.identity);
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(lightningClip.length);
        bossZ.ligtningSpellInProgress = false;
    }
}

public class PassToPhaseTwoStrategy : MainStrategyForBossZ, IStrategy
{
    AnimatorStateInfo stateInfo;
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("PassToPhaseTwoStrategy");
        stateInfo = bossZ.bossAnim.GetCurrentAnimatorStateInfo(0);
        bossZ.invincible = true;
        if (stateInfo.IsName("passToPhaseTwo"))
        {
            if(stateInfo.normalizedTime >= 1 )
            {
                bossZ.phaseTwo = false;
                bossZ.phaseTwoIdle = true;
                bossZ.inPhaseTwo = true;
                bossZ.invincible = false;
                return Node.NodeStatus.SUCCESS;
            }
        }

        return Node.NodeStatus.RUNNING;
       
    }
}

public class CreateShieldStrategy : MainStrategyForBossZ, IStrategy
{
    AnimatorStateInfo stateInfo;
    public Node.NodeStatus Evaluate()
    {
        Debug.Log("Create Shield Strategy");
        stateInfo = bossZ.bossAnim.GetCurrentAnimatorStateInfo(0);
        bossZ.createShield = true;
        if (stateInfo.IsName("shield"))
        {
            if (bossZ.summonedBeings.Count == 0)
            {
                bossZ.createShield = false;
                bossZ.canSummon = false;
                return Node.NodeStatus.SUCCESS;
            }
        }

        return Node.NodeStatus.RUNNING;

    }
}


public class SummonStrategy : MainStrategyForBossZ, IStrategy
{
    Boss reference;
    Boss summon;
    ParticleSystem summonParticle;
    bool summonProgressed = false;
    float timeToPass;

    public SummonStrategy(Boss summonedThing, ParticleSystem summonParticle)
    {
        this.reference = summonedThing;
        this.summonParticle = summonParticle;
    }

    public Node.NodeStatus Evaluate()
    {
        if (!summonProgressed)
        {
            timeToPass = summonParticle.main.duration;
            summon = GameObject.Instantiate(reference);
            summon.OnSummon(bossZ.summonPosition.transform.position, bossZ, summonParticle);
            summonProgressed = true;
        }
        timeToPass -= Time.deltaTime;
        if(timeToPass <= 0)
        {
            summonProgressed = false;
            summon.chase = true;
            return Node.NodeStatus.SUCCESS;  
        }
        return Node.NodeStatus.RUNNING;
    }
}



