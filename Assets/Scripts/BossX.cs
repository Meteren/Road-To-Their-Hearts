using AdvancedStateHandling;
using Cinemachine;
using System.Collections;
using UnityEngine;

public class BossX : Boss
{
    
    [SerializeField] private GameObject panel;

    public float duration;

    [Header("Dialogue States")] 
    public bool specialOneProgressed = false;
    public bool specialTwoProgressed = false;
    public bool firstEncounterReady = true;
    public bool inSpecialOneToSayIsReady = false;
    public bool inSpecialTwoToSayIsReady = false;

    [Header("Conditions")]
    
    public bool isJumped;
    public bool canAttack;
    public bool isDashReady = false;
    public bool isStanceReady = false;
    public bool isRandomLongRangeInProgress = false;
    public bool isNormalDashAttackInProgress = false;
    public bool isDashAttackInProgress = false;
    public bool isUpThere = false;
    public bool onLand = false;
    public bool increaseProbOfDashAttack = false;
    public bool initBossXSequence = false;

    [Header("Special Attack Conditions")]
    public bool specialOneCoroutineBlocker = false;
    public bool specialTwoCoroutineBlocker = false;
    public bool isSpecialOneReady = false;
    public bool isSpecialTwoReady = false;
    public bool inSpecialTwo = false;


    [HideInInspector] public float probOfLongRangeAttack;
    [HideInInspector] public float probOfSpecialOneAttack;
    [HideInInspector] public float probOfSpecialTwoAttack;
    [HideInInspector] public float probOfDashAttack;


    [SerializeField] private float distanceToGround;
    [SerializeField] private float distanceToRight;
    [SerializeField] private float distanceToLeft;

    [Header("WayPoints")]
    public Transform wayPointRight;
    public Transform wayPointLeft;
    public Transform upPoint;
    public Transform downPoint;
    public Transform offsetWayPoint;

    float bossXDistanceToLeftWayPoint => Vector2.Distance(transform.position, wayPointLeft.transform.position);
    float bossXDistanceToRightWayPoint => Vector2.Distance(transform.position, wayPointRight.transform.position);


    float centerPoint => (Vector2.Distance(wayPointLeft.transform.position, wayPointRight.transform.position)) / 2;

    public float defaultGravity;
    public ParticleSystem groundImpactParticle;
    

    [Header("Dialogues")]
    [SerializeField] private DialogueContainer firstEncounter;
    [SerializeField] private DialogueContainer inSepcialOneToSay;
    [SerializeField] private DialogueContainer inSpecialTwoToSay;
    [SerializeField] private DialogueContainer inDeathToSay;
    [SerializeField] private DialogueContainer inCharacterDeathToSay;

    private void Start()
    {
        
        currentHealth = maxHealth;
        bossHealthBar.SetMaxHealth(maxHealth);
        
        defaultGravity = rb.gravityScale;
        IgnoreCollision();

        blackBoard.SetValue("BossX", this);

        InitBehaviourTree();

        SortedSelectorNode mainSelector = new SortedSelectorNode("MainSelector");
        SortedSelectorNode selectJumpType = new SortedSelectorNode("SelectJumpType");
        SequenceNode farestWayPointJumpSequence = new SequenceNode("FarestWayPointJumpSequence",10);
        SequenceNode closestWayPointJumpSequence = new SequenceNode("ClosestWayPointJumpSequence",20);

        Leaf farestWayPointJumpCondition = new Leaf("FarestWayPointJumpCondition", new Condition(() => IsTooCloseToAWayPoint()));
        Leaf closestWayPointJumpCondition = new Leaf("ClosestWayPointJumpCondition", new Condition(() => !IsTooCloseToAWayPoint()));
        Leaf farestWayPointJumpStrategy = new Leaf("FarestWayPointJumpStrategy",new JumpToAWayPointStrategy(JumpToAWayPointStrategy.Method.farest));
        Leaf closestWayPointJumpStrategy = new Leaf("ClosestWayPointJumpStrategy", new JumpToAWayPointStrategy(JumpToAWayPointStrategy.Method.closest));

        SequenceNode dieSequence = new SequenceNode("DieSequence",2);
        Leaf dieCondition = new Leaf("DieCondition", new Condition(() => isDead));
        Leaf dieStrategy = new Leaf("DieStrategy", new DieStrategy(this));

        SequenceNode doNothingSequence = new SequenceNode("DoNothingSequence", 1);
        Leaf doNothingCondition = new Leaf("DoNothingCondition", new Condition(() => playerController.isDead));
        Leaf doNothingStrategy = new Leaf("DoNothingStrategy", new DoNothingStrategy(this));

        SortedSelectorNode selectSpecialAttack = new SortedSelectorNode("SelectSpecialAttack",5);
        SequenceNode specialAttackOneSequence = new SequenceNode("SpecialAttackOneSequence", 10);
        SequenceNode specialAttackTwoSequence = new SequenceNode("SpecialAttackTwoSequence",5);

        Leaf specialAttackTwoReady = new Leaf("SpecialAttackTwoReady", new Condition(() => {
             if (!specialTwoCoroutineBlocker)
             {
                StartCoroutine(GenerateNumberForSpecialAttackTwo());
             }
             return (probOfSpecialTwoAttack > 55) && !firstEncounterReady && isSpecialTwoReady;
        }));
        SequenceNode startSpecialAttackTwoSequence = new SequenceNode("StartSpecialAttackTwoSequence");

        Leaf jumpAboveStrategy = new Leaf("JumpAboveStrategy", new JumpAboveStrategy());
        Leaf landAndInflictDamageStrategy = new Leaf("LandAndInflictDamage", new LandAndInflictDamageStrategy());

        SequenceNode normalDashAttackSequence = new SequenceNode("NormalDashAttackSequence", 5);
        Leaf normalDashAttackCondition = new Leaf("NormalDashaAttackCondition", new Condition(() =>
        {
            if (!isNormalDashAttackInProgress)
            {
                StartCoroutine(GenerateNumberForDashAttack());
            }

            return probOfDashAttack > (isSpecialOneReady ? (increaseProbOfDashAttack ? 30 : 50) : 70) && distanceToPlayer > 4.6f && !firstEncounterReady;

        }));
        Leaf normalDashAttackStrategy = new Leaf("NormalDashAttackStrategy", new DashAttackStrategy(false));

        SequenceNode normalCloseRangeAttackSequence = new SequenceNode("CloseRangeAttackSequence", 20);
        SequenceNode specialCloseRangeAttackSequence = new SequenceNode("SpecialCloseRangeAttackSequence", 20);
        SequenceNode randomLongRangeAttackSequence = new SequenceNode("RandomLongRangeAttackSequence", 10);
        SequenceNode specialLongRangeAttackSequence = new SequenceNode("SpecialLongRangeAttackSequence",10);
        SortedSelectorNode selectNormalAttacks = new SortedSelectorNode("SelectNormalAttacks", 10);
        SequenceNode chaseSequence = new SequenceNode("ChaseSequence", 20);

        Leaf stayStillStrategy = new Leaf("StayStill", new StayStillStrategy(this), 30);
        Leaf chaseCondition = new Leaf("ChaseCondition", new Condition(() => !firstEncounterReady));
        Leaf chasePlayerStrategy = new Leaf("ChasePlayerStrategy", new ChasePlayerStrategy(this,4));
        Leaf closeRangeAttackCondition = new Leaf("AttackOneCondition", new Condition(() =>
                    distanceToPlayer < 2.4f && !isInCloseRangeAttack));
        Leaf normalCloseRangeAttackStrategy = new Leaf("NormalCloseRangeAttackStrategy", new CloseRangeAttackStrategyForBossX(false));
        Leaf specialCloseRangeAttackStrategy = new Leaf("SpecialCloseRangeAttackStrategy", new CloseRangeAttackStrategyForBossX(true));
        
        Leaf randomLongRangeAttackCondition = new Leaf("LongeRangeAttackCondition", new Condition(() =>
        {
            if(!isRandomLongRangeInProgress)
                StartCoroutine(GenerateNumberForLongRangeAttack());
            return ((distanceToPlayer > 2.4f && distanceToPlayer < 4.6f) && (probOfLongRangeAttack > 60 && probOfLongRangeAttack < 100));
        }));

        Leaf longRangeAttackCondition = new Leaf("LongeRangeAttackCondition", new Condition(() => (distanceToPlayer > 2.4f && distanceToPlayer < 5.6f)));

        Leaf normalLongRangeAttackStrategy = new Leaf("NormalLongRangeAttackStrategy", new LongRangeAttackStrategyForBossX(false));
        Leaf specialLongRangeAttackStrategy = new Leaf("SpecialLongRangeAttackStrategy", new LongRangeAttackStrategyForBossX(true));

        Leaf specialDashAttackStrategy = new Leaf("AttackAfterDashStrategy", new DashAttackStrategy(true));

        Leaf dashAttackCondition = new Leaf("DashAttackCondition", new Condition(() => distanceToPlayer > 5.6f));

        SequenceNode specialDashAttackSequence = new SequenceNode("SpecialDashAttackSequence",5);
  
        Leaf specialAttackOneReady = new Leaf("SpecialAttackOneReady", new Condition(() =>
        {
            if (!specialOneCoroutineBlocker)
            {
                StartCoroutine(GenerateNumberForSpecialAttackOne());
            }            
            return (probOfSpecialOneAttack > 75) && !firstEncounterReady && isSpecialOneReady;  
        }));
        
        SequenceNode startSpecialOneSequence = new SequenceNode("StartSpecialOneSequence");
       
        SortedSelectorNode selectAttackAfterJump = new SortedSelectorNode("SelectAttackAfterJump");
       
        //Special attack one sequence
        specialAttackOneSequence.AddChild(specialAttackOneReady);
        specialAttackOneSequence.AddChild(startSpecialOneSequence);

        //start special one sequence
        startSpecialOneSequence.AddChild(selectJumpType);
        startSpecialOneSequence.AddChild(selectAttackAfterJump);

        //add jump types
        selectJumpType.AddChild(farestWayPointJumpSequence);
        selectJumpType.AddChild(closestWayPointJumpSequence);

        //ready jump sequences
        farestWayPointJumpSequence.AddChild(farestWayPointJumpCondition);
        farestWayPointJumpSequence.AddChild(farestWayPointJumpStrategy);
        closestWayPointJumpSequence.AddChild(closestWayPointJumpCondition);
        closestWayPointJumpSequence.AddChild(closestWayPointJumpStrategy);

        //attack style selector after jump
        selectAttackAfterJump.AddChild(specialDashAttackSequence);
        selectAttackAfterJump.AddChild(specialLongRangeAttackSequence);
        selectAttackAfterJump.AddChild(specialCloseRangeAttackSequence);

        //special close range attack sequence
        specialCloseRangeAttackSequence.AddChild(closeRangeAttackCondition);
        specialCloseRangeAttackSequence.AddChild(specialCloseRangeAttackStrategy);

        //normal close range attack sequence
        normalCloseRangeAttackSequence.AddChild(closeRangeAttackCondition);
        normalCloseRangeAttackSequence.AddChild(normalCloseRangeAttackStrategy);

        //dash attack sequence
        specialDashAttackSequence.AddChild(dashAttackCondition);
        specialDashAttackSequence.AddChild(specialDashAttackStrategy);

        //random long range attack sequence
        randomLongRangeAttackSequence.AddChild(randomLongRangeAttackCondition);
        randomLongRangeAttackSequence.AddChild(normalLongRangeAttackStrategy);

        //special long range attack Sequence
        specialLongRangeAttackSequence.AddChild(longRangeAttackCondition);
        specialLongRangeAttackSequence.AddChild(specialLongRangeAttackStrategy);

        //Chase Player Sequence
        chaseSequence.AddChild(chaseCondition);
        chaseSequence.AddChild(chasePlayerStrategy);

        //randomized normal dash attack
        normalDashAttackSequence.AddChild(normalDashAttackCondition);
        normalDashAttackSequence.AddChild(normalDashAttackStrategy);

        //Choose attack between long range, close range, and dash attack
        selectNormalAttacks.AddChild(normalDashAttackSequence);
        selectNormalAttacks.AddChild(randomLongRangeAttackSequence);
        selectNormalAttacks.AddChild(normalCloseRangeAttackSequence);

        //Start special attack two sequence
        startSpecialAttackTwoSequence.AddChild(jumpAboveStrategy);
        startSpecialAttackTwoSequence.AddChild(landAndInflictDamageStrategy);

        //Special attack two sequence
        specialAttackTwoSequence.AddChild(specialAttackTwoReady);
        specialAttackTwoSequence.AddChild(startSpecialAttackTwoSequence);

        //Select Special Attacks
        selectSpecialAttack.AddChild(specialAttackTwoSequence);
        selectSpecialAttack.AddChild(specialAttackOneSequence);

        //handling death
        dieSequence.AddChild(dieCondition);
        dieSequence.AddChild(dieStrategy);

        //player death
        doNothingSequence.AddChild(doNothingCondition);
        doNothingSequence.AddChild(doNothingStrategy);

        //Main Selector
        mainSelector.AddChild(doNothingSequence);
        mainSelector.AddChild(dieSequence);
        mainSelector.AddChild(selectSpecialAttack); 
        mainSelector.AddChild(selectNormalAttacks);
        mainSelector.AddChild(chaseSequence);
        mainSelector.AddChild(stayStillStrategy);
       
        //BossX Behaviour Tree
        bossBehaviourTree.AddChild(mainSelector);

        var noDialogState = new NoDialogueState(this, panel);
        var firstEncaounterToSay = new FirstEncounterToSay(this, firstEncounter, panel);
        var inSpecialOne = new InSpecialOneToSay(this, inSepcialOneToSay, panel);
        var inSpecialTwo = new InSpecialTwoToSay(this, inSpecialTwoToSay, panel);
        var inDeath = new InDeathToSay(inDeathToSay,this,panel);
        var inCharacterDeath = new InCharacterDeathToSay(inCharacterDeathToSay, this, panel);
        

        dialogueStateMachine.currentState = noDialogState;

        At(noDialogState, firstEncaounterToSay, new FuncPredicate(() => firstEncounterReady));
        At(firstEncaounterToSay, noDialogState, new FuncPredicate(() => !firstEncounterReady));
        At(noDialogState, inSpecialOne, new FuncPredicate(() => inSpecialOneToSayIsReady));
        At(inSpecialOne, noDialogState, new FuncPredicate(() => !inSpecialOneToSayIsReady));
        At(noDialogState, inSpecialTwo, new FuncPredicate(() => inSpecialTwoToSayIsReady));
        At(inSpecialTwo, noDialogState, new FuncPredicate(() => !inSpecialTwoToSayIsReady));
        At(noDialogState, inDeath, new FuncPredicate(() => inDeathToSayIsReady));
        At(inDeath, noDialogState, new FuncPredicate(() => !inDeathToSayIsReady));
        At(noDialogState, inCharacterDeath, new FuncPredicate(() => inCharacterDeathToSayIsReady));
        At(inCharacterDeath, noDialogState, new FuncPredicate(() => !inCharacterDeathToSayIsReady));

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x,transform.position.y - distanceToGround));
        Gizmos.DrawLine(upPoint.position, new Vector2(upPoint.position.x + distanceToRight,upPoint.position.y));
        Gizmos.DrawLine(upPoint.position, new Vector2(upPoint.position.x - distanceToLeft, upPoint.position.y));
    }


    private void CollisionDetection()
    {
        //isJumped = !Physics2D.Raycast(transform.position, Vector2.down,distanceToGround,ground);
        isUpThere = Physics2D.Raycast(upPoint.position, Vector2.left, distanceToLeft, bossLayer) ||
            Physics2D.Raycast(upPoint.position, Vector2.right, distanceToRight, bossLayer);
    }


    private void At(IState from, IState to, IPredicate condition)
    {
        dialogueStateMachine.AddTransition(from, to, condition);
    }

    private void FixedUpdate()
    {
        bossBehaviourTree.Process();
        CollisionDetection();
    }
    private void Update()
    {
        Debug.Log(currentHealth);
        if (currentHealth < 65)
        {
            isSpecialOneReady = true;
            if (!specialOneProgressed)
            {
                inSpecialOneToSayIsReady = true;
                specialOneProgressed = true;
            }
                
        }

        if (currentHealth < 40)
        {
            isSpecialTwoReady = true;
            if (!specialTwoProgressed)
            {
                inSpecialTwoToSayIsReady = true;
                specialTwoProgressed = true;
            }
            
        }

        if (currentHealth < 30)
        {
            increaseProbOfDashAttack = true;
        }

        SetDireaction();
        if (currentHealth <= 0)
        {
            isDead = true; 

        }

        if (Input.GetKeyDown(KeyCode.L))
            specialTwoCoroutineBlocker = true;

        Debug.Log("special two ready:" + specialTwoCoroutineBlocker);

        if (!bossAnim.GetCurrentAnimatorStateInfo(0).IsName("CloseRangeAttack")
            && !bossAnim.GetCurrentAnimatorStateInfo(0).IsName("LongRangeAttack")
            && !bossAnim.GetCurrentAnimatorStateInfo(0).IsName("AttackAfterDash")
            && !bossAnim.GetCurrentAnimatorStateInfo(0).IsName("OnLand") && !isDead)
            SetRotation();

        xVelocity = rb.velocity.x;
        if(GameManager.instance.init)
            dialogueStateMachine.Update();
        AnimationController();
    }
    public override void AnimationController()
    {
        bossAnim.SetFloat("xVelocity", xVelocity);
        bossAnim.SetFloat("yVelocity", yVelocity);

        bossAnim.SetBool("isJumped", isJumped);
        bossAnim.SetBool("isStanceReady", isStanceReady);
        bossAnim.SetBool("isInLongRangeAttack", isInLongRangeAttack);
        bossAnim.SetBool("isInCloseRangeAttack", isInCloseRangeAttack);
        bossAnim.SetBool("onLand", onLand);
        bossAnim.SetBool("isDead", isDead);
        bossAnim.SetBool("inSpecialTwo", inSpecialTwo);
    }

    public void StartShakingCamera()
    {
        StartCoroutine(ShakeCamera());
    }

    private IEnumerator ShakeCamera()
    {
        CinemachineBasicMultiChannelPerlin channel = 
            blackBoard.GetValue("Channel", out CinemachineBasicMultiChannelPerlin _channel) ? _channel : null;
        channel.m_AmplitudeGain = 2.5f;
        yield return new WaitForSeconds(0.5f);
        channel.m_AmplitudeGain = 0;
    }

    private IEnumerator GenerateNumberForDashAttack()
    {
        probOfDashAttack = Random.Range(0, 100);
        isNormalDashAttackInProgress = true;
        yield return new WaitForSeconds(1f);
        isNormalDashAttackInProgress = false;
    }

    private IEnumerator GenerateNumberForLongRangeAttack()
    {
        probOfLongRangeAttack = Random.Range(0, 100);
        isRandomLongRangeInProgress = true;
        yield return new WaitForSeconds(1f);
        isRandomLongRangeInProgress = false;
    }

    private IEnumerator GenerateNumberForSpecialAttackOne()
    {
        probOfSpecialOneAttack = Random.Range(0, 100);
        specialOneCoroutineBlocker = true;
        yield return new WaitForSeconds(0.5f);
        specialOneCoroutineBlocker = false;
    }

    private IEnumerator GenerateNumberForSpecialAttackTwo()
    {
        probOfSpecialTwoAttack = Random.Range(0, 100);
        specialTwoCoroutineBlocker = true;
        yield return new WaitForSeconds(2f);
        specialTwoCoroutineBlocker = false;
    }

    public void EndProgress()
    {
        Debug.Log("Progress Ended");
        isDashAttackInProgress = false;
    }

    private bool IsTooCloseToAWayPoint()
    {
        if(((bossXDistanceToLeftWayPoint < 2f) 
            && (playerController.transform.position.x < (wayPointLeft.transform.position.x + centerPoint))) 
            || ((bossXDistanceToRightWayPoint < 2f) 
            && (playerController.transform.position.x > (wayPointRight.transform.position.x - centerPoint))))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void OnDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        bossHealthBar.SetCurrentHealth(currentHealth);
    }

    public override float InflictDamage()
    {
        float inflictedDamage = 10f;
        return inflictedDamage;
    }

    public override void InitBehaviourTree()
    {
        bossBehaviourTree = new BehaviourTree("BossXBehaviourTree");
    }

}
