using AdvancedStateHandling;
using System.Collections;
using UnityEngine;

public class BossZ : Boss
{
    private BehaviourTree bossZTree;
    public Transform centerPoint;

    float demonSpellDistance = 6f;
    float dragonSpellDistance = 3f;
    float healthBorderForSummon = 40;

    int probOfCloseRangeAttack;
   
    int probOfLongRangeAttack;

    int probOfFireLightning;

    [Header("GenerationFrame")]
    public Collider2D generationFrame;

    [Header("SummonParticle")]
    public ParticleSystem summonParticle;

    [Header("Spells")] 
    public Spell demonSpell;
    public Spell darkSpell;
    public Spell blueDragon;
    public Spell redDragon;
    public Spell fireLightning;


    [Header("Being To Summon")]
    public NightBorne nightborne;

    [Header("Waypoints")]
    public Transform demonSpellWayPointOne;
    public Transform demonSpellWayPointTwo;
    public Transform dragonSpellWayPointRight;
    public Transform dragonSpellWayPointLeft;
    public Transform fireLightningPos;

    [Header("Conditions")]
    public bool canMeleeAttack;
    public bool firstReadyToSay = false;
    public bool phaseOneCastSpell;
    public bool phaseTwoCastSpell;
    public bool createShield;
    public bool blockCoroutineForCloseRangeAttack = false;
    public bool blockCoroutineForLongRangeAttack = false;
    public bool blockFireLightningCoroutine = false;
    public bool activateDemonSpell = false;
    public bool activateDragonSpell = false;
    private bool collisionIgnored = false;
    public bool inRangeOfLightning = false;
    public bool lightningActive = false;
    public bool meleeSequence = false;
    public bool canSummon = false;

    [Header("SpellProgressions")]
    public bool demonSpellInProgress = false;
    public bool dragonSpellInProgress = false;
    public bool ligtningSpellInProgress = false;

    float playerDistanceToLeft => Vector2.Distance(playerController.transform.position, demonSpellWayPointOne.transform.position);

    float playerDistanceToRight => Vector2.Distance(playerController.transform.position, demonSpellWayPointTwo.transform.position);


    void Start()
    {
        currentHealth = maxHealth;

        GameManager.instance.blackBoard.SetValue("BossZ", this);

        InitBehaviourTree();
 
        SortedSelectorNode mainSelector = new SortedSelectorNode("MainSelector");

        Leaf stayStillStrategy = new Leaf("StayStillStrategy", new StayStillStrategy(this),40);


        SortedSelectorNode attackSelector = new SortedSelectorNode("AttackSelector", 30);

        SequenceNode meleeAttackSequence = new SequenceNode("MeleeAttackSequence",20);

        attackSelector.AddChild(meleeAttackSequence);
        
        Leaf meleeAttackCondition = new Leaf("MeleeCondition",new Condition(() =>
        {
            if (!blockCoroutineForCloseRangeAttack)
            {
                StartCoroutine(GenerateNumberForCloseRangeAttack());
            }

            return probOfCloseRangeAttack > 35 && !inPhaseTwo && !firstEncounterReady;
        }));

        meleeAttackSequence.AddChild(meleeAttackCondition);
        
        SequenceNode processAttackSequence = new SequenceNode("ProcessAttackSequence");
        meleeAttackSequence.AddChild(processAttackSequence);

        Leaf moveToPlayerStrategy = new Leaf("MoveToPlayerStrategy",
            new HandleMovementStrategy(HandleMovementStrategy.State.moveToPlayerOffset, Vector2.zero, 35f));
        Leaf attackStrategy = new Leaf("AttackStrategy", new MeleeAttackStrategy());
        Leaf getBackStrategy =
            new Leaf("GetBackStrategy",
            new HandleMovementStrategy(HandleMovementStrategy.State.moveToChoosedPos, centerPoint.transform.position, 35f));

        SequenceNode darkSpellSequence = new SequenceNode("DarkSpellSequence",10);
        attackSelector.AddChild(darkSpellSequence);

        Leaf darkSpellCondition = new Leaf("DarkSpellCondition", new Condition(() =>
        {
            if (!blockCoroutineForLongRangeAttack)
            {
                StartCoroutine(GenerateNumberForLongRangeAttack());
            }
            return probOfLongRangeAttack > 60 && !firstEncounterReady;
        }));

        Leaf castSpellStrategy = new Leaf("CastSpellStrategy", new CastSpellStrategy());

        Leaf darkSpellStrategy = new Leaf("DarkSpellStrategy", new DarkSpellStrategy(darkSpell));

        darkSpellSequence.AddChild(darkSpellCondition);
        darkSpellSequence.AddChild(castSpellStrategy);
        darkSpellSequence.AddChild(darkSpellStrategy);

        SortedSelectorNode spellSelector = new SortedSelectorNode("SpellSelector",25);

        SequenceNode demonSpellSequence = new SequenceNode("DemonSpellSequence", 25);

        spellSelector.AddChild(demonSpellSequence);

        Leaf demonSpellCondition = new Leaf("DemonSpellCondition", new Condition(() => 
        (playerDistanceToLeft < demonSpellDistance || playerDistanceToRight < demonSpellDistance) && !demonSpellInProgress && activateDemonSpell));

        Leaf demonSpellStrategy = new Leaf("DemonSpellStrategy", new DemonSpellStrategy(demonSpell));

        SequenceNode dragonSpellSequence = new SequenceNode("DragonSpellSequence", 20);

        spellSelector.AddChild(dragonSpellSequence);

        Leaf dragonSpellCondition = new Leaf("DragonSpellCondition",new Condition(()=>
        (playerDistanceToLeft < dragonSpellDistance || playerDistanceToRight < dragonSpellDistance) && !dragonSpellInProgress && activateDragonSpell));
        Leaf dragonSpellStrategy = new Leaf("DragonSpellCondition", new DragonSpellStrategy(blueDragon, redDragon));

        SequenceNode fireLightningSpellSequence = new SequenceNode("FireSpellSequence",5);

        spellSelector.AddChild(fireLightningSpellSequence);

        Leaf fireLightningCondition = new Leaf("FireLightningCondition", new Condition(() =>
        {
            if (!blockFireLightningCoroutine)
            {
                StartCoroutine(GenerateNumberForFireLightning());
            }

            return probOfFireLightning > 40 && !ligtningSpellInProgress && inRangeOfLightning && lightningActive;
        }));
        Leaf fireLightningStrategy = new Leaf("FireLightningStrategy", new FireLightningSpellStrategy(fireLightning,fireLightningPos));

        SequenceNode passToPhaseTwoSequence = new SequenceNode("PassToPhaseTwoSequence", 10);

        Leaf phaseTwoCondition = new Leaf("PhaseTwoCondition", new Condition(() => phaseTwo));
        Leaf passToPhaseTwoStrategy = new Leaf("PassToPhaseTwoStrategy", new PassToPhaseTwoStrategy());

        SequenceNode summonSequence = new SequenceNode("SummonSequence",15);

        Leaf summonCondition = new Leaf("SummonCondition", new Condition(() => canSummon));

        Leaf summonStrategy = new Leaf("SummonStrategy", new SummonStrategy(nightborne,summonParticle));

        Leaf createShieldStrategy = new Leaf("CreateShieldStrategy", new CreateShieldStrategy());

        SequenceNode dieSequence = new SequenceNode("DieSequence", 11);

        Leaf dieCondition = new Leaf("DieCondition", new Condition(() => isDead));
        Leaf dieStrategy = new Leaf("DieStrategy", new DieStrategy(this));

        SequenceNode doNothingSequence = new SequenceNode("DoNothingSequence", 12);
        Leaf doNothingCondition = new Leaf("DoNothingCondition", new Condition(() => playerController.isDead));
        Leaf doNothingStrategy = new Leaf("DoNothingStrategy", new DoNothingStrategy(this));

        doNothingSequence.AddChild(doNothingCondition);
        doNothingSequence.AddChild(doNothingStrategy);

        dieSequence.AddChild(dieCondition);
        dieSequence.AddChild(dieStrategy);

        summonSequence.AddChild(summonCondition);
        summonSequence.AddChild(castSpellStrategy);
        summonSequence.AddChild(summonStrategy);
        summonSequence.AddChild(createShieldStrategy);
        
        passToPhaseTwoSequence.AddChild(phaseTwoCondition);
        passToPhaseTwoSequence.AddChild(passToPhaseTwoStrategy);

        fireLightningSpellSequence.AddChild(fireLightningCondition);
        fireLightningSpellSequence.AddChild(castSpellStrategy);
        fireLightningSpellSequence.AddChild(fireLightningStrategy);

        dragonSpellSequence.AddChild(dragonSpellCondition);
        dragonSpellSequence.AddChild(castSpellStrategy);
        dragonSpellSequence.AddChild(dragonSpellStrategy);

        demonSpellSequence.AddChild(demonSpellCondition);
        demonSpellSequence.AddChild(castSpellStrategy);
        demonSpellSequence.AddChild(demonSpellStrategy);

        processAttackSequence.AddChild(moveToPlayerStrategy);
        processAttackSequence.AddChild(attackStrategy);
        processAttackSequence.AddChild(getBackStrategy);

        mainSelector.AddChild(stayStillStrategy);
        mainSelector.AddChild(attackSelector);
        mainSelector.AddChild(spellSelector);
        mainSelector.AddChild(summonSequence);
        mainSelector.AddChild(doNothingSequence);
        mainSelector.AddChild(dieSequence);
        mainSelector.AddChild(passToPhaseTwoSequence);

        bossZTree.AddChild(mainSelector);

        var noDialogState = new NoDialogueState(this, panel);
        var firstEncaounterToSay = new FirstEncounterToSay(this, firstEncounter, panel);
        var inSpecialOne = new InSpecialOneToSay(this, inSepcialOneToSay, panel);
        var inSpecialTwo = new InSpecialTwoToSay(this, inSpecialTwoToSay, panel);
        var inDeath = new InDeathToSay(inDeathToSay, this, panel);
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

    private void At(AdvancedStateHandling.IState from, AdvancedStateHandling.IState to, IPredicate condition)
    {
        dialogueStateMachine.AddTransition(from, to, condition);
    }

    private void FixedUpdate()
    {
        bossZTree.Process();
    }
    void Update()
    {
        CheckRangeOfPlayer();
        if (!collisionIgnored)
        {
            collisionIgnored = true;
            IgnoreCollision();
            IgnoreGroundCollisiton();
        }
        if(currentHealth < 85)
        {
            activateDemonSpell = true;
        }

        if(currentHealth < 70)
        {
            activateDragonSpell = true;
        }

        SetDireaction();

        if (!bossAnim.GetCurrentAnimatorStateInfo(0).IsName("melee_attack"))
            SetBossZRotation();

        AnimationController();

        if(currentHealth < 50 && !inPhaseTwo)
        {
            phaseTwo = true;
            lightningActive = true;
            inSpecialOneToSayIsReady = true;

        }

        if(currentHealth < healthBorderForSummon && inPhaseTwo)
        {
            canSummon = true;
            healthBorderForSummon -= 20;
            inSpecialTwoToSayIsReady = true;
        }

        if(currentHealth <= 0)
        {
            isDead = true;
        }

        dialogueStateMachine.Update();


    }

    private void CheckRangeOfPlayer()
    {
        float minX = generationFrame.bounds.min.x;
        float maxX = generationFrame.bounds.max.x;
        if (playerController.transform.position.x > minX && playerController.transform.position.x < maxX)
        {
            inRangeOfLightning = true;
        }
        else
        {
            inRangeOfLightning = false;
        }
    }

    private void SetBossZRotation()
    {
        if (direction.x > 0 && !isFacingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            isFacingLeft = true;
        }

        if (direction.x < 0 && isFacingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            isFacingLeft = false;
        }
    }
    public override void AnimationController()
    {
        bossAnim.SetBool("meleeAttack", canMeleeAttack);
        bossAnim.SetBool("castSpell", phaseOneCastSpell);
        bossAnim.SetBool("createShield", createShield);
        bossAnim.SetBool("passToPhaseTwo", phaseTwo);
        bossAnim.SetBool("phaseTwoIdle", phaseTwoIdle);
        bossAnim.SetBool("phaseTwoCastSpell", phaseTwoCastSpell);
        bossAnim.SetBool("meleeSequence", meleeSequence);
        bossAnim.SetBool("isDead", isDead);
    }

    public override float InflictDamage()
    {
        return 2f;
    }

    public override void InitBehaviourTree()
    {
        bossZTree = new BehaviourTree("BossZ");
    }

    public override void OnDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        bossHealthBar.SetCurrentHealth(currentHealth);
   
    }

    private IEnumerator GenerateNumberForCloseRangeAttack()
    {
        probOfCloseRangeAttack = Random.Range(0, 100);
        blockCoroutineForCloseRangeAttack = true;
        yield return new WaitForSeconds(1f);
        blockCoroutineForCloseRangeAttack = false;

    }

    private IEnumerator GenerateNumberForLongRangeAttack()
    {
        probOfLongRangeAttack = Random.Range(0, 100);
        blockCoroutineForLongRangeAttack = true;
        yield return new WaitForSeconds(1f);
        blockCoroutineForLongRangeAttack = false;
    }

    private IEnumerator GenerateNumberForFireLightning()
    {
        probOfFireLightning = Random.Range(0, 100);
        blockFireLightningCoroutine = true;
        yield return new WaitForSeconds(1f);
        blockFireLightningCoroutine = false;
    }

    private void IgnoreGroundCollisiton()
    {
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"));
    }
}
