using System.Collections;
using UnityEngine;

public class BossZ : Boss
{
    private BehaviourTree bossZTree;
    public Transform centerPoint;

    float demonSpellDistance = 6f;
    float dragonSpellDistance = 3f;

    int probOfCloseRangeAttack;
   
    int probOfLongRangeAttack;

    [Header("Spells")] 
    public Spell demonSpell;
    public Spell darkSpell;
    public Spell blueDragon;
    public Spell redDragon;

    [Header("Waypoints")]
    public Transform demonSpellWayPointOne;
    public Transform demonSpellWayPointTwo;
    public Transform dragonSpellWayPointRight;
    public Transform dragonSpellWayPointLeft;

    [Header("Conditions")]
    public bool canMeleeAttack;
    public bool firstReadyToSay = false;
    public bool castSpell;
    public bool createShield;
    public bool blockCoroutineForCloseRangeAttack = false;
    public bool blockCoroutineForLongRangeAttack = false;
    public bool activateDemonSpell = false;
    public bool activateDragonSpell = false;
    private bool collisionIgnored = false;

    [Header("SpellProgressions")]
    public bool demonSpellInProgress = false;
    public bool dragonSpellInProgress = false;

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

            return probOfCloseRangeAttack > 35;
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
            return probOfLongRangeAttack > 60;
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

        bossZTree.AddChild(mainSelector);


    }

    private void FixedUpdate()
    {
        bossZTree.Process();
    }
    void Update()
    {
        if (!collisionIgnored)
        {
            collisionIgnored = true;
            IgnoreCollision();
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

        if (Input.GetKeyDown(KeyCode.B))
        {
            firstReadyToSay = !firstReadyToSay;
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
        bossAnim.SetBool("castSpell", castSpell);
        bossAnim.SetBool("createShield", createShield);
    }

    public override float InflictDamage()
    {
        throw new System.NotImplementedException();
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
}
