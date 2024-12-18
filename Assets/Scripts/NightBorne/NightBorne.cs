using UnityEngine;

public class NightBorne : Boss
{
    [Header("Individual Conditions")]
    public bool attack;
    Boss belongsTo;
    float individualHealth = 50;

    private void Start()
    {
        IgnoreCollision();
        currentHealth = individualHealth;
        bossHealthBar.SetMaxHealth(currentHealth);
        bossHealthBar.SetCurrentHealth(currentHealth);
        GameManager.instance.blackBoard.SetValue("NightBorne", this);
        InitBehaviourTree();

        SortedSelectorNode mainSelectorNode = new SortedSelectorNode("MainSelectorNode");

        Leaf stayStillStrategy = new Leaf("StayStillStrategy", new StayStillStrategy(this),30);

        SequenceNode chaseSequence = new SequenceNode("ChaseSequence", 25);

        Leaf chaseCondition = new Leaf("ChaseCondition", new Condition(() => chase));
        Leaf chaseStrategy = new Leaf("ChaseStrategy", new ChasePlayerStrategy(this,15));
        
        chaseSequence.AddChild(chaseCondition);
        chaseSequence.AddChild(chaseStrategy);

        SequenceNode attackSequence = new SequenceNode("AttackSequence", 20);

        Leaf attackCondition = new Leaf("AttackCondition", new Condition(() => distanceToPlayer < 4f));
        Leaf attackStrategy = new Leaf("AttackStrategy", new AttackStrategy());


        attackSequence.AddChild(attackCondition);
        attackSequence.AddChild(attackStrategy);

        SequenceNode inDeathSequence = new SequenceNode("InDeathsSequence",10);

        Leaf deathCondition = new Leaf("DeathCondition", new Condition(() => isDead));
        Leaf inDeathStrategy = new Leaf("InDeathStrategy", new InDeathStrategy());

        SequenceNode doNothingSequence = new SequenceNode("DoNothing", 5);

        Leaf doNothingCondition = new Leaf("DoNothingCondition",new Condition(()=>playerController.isDead));
        Leaf doNothingStrategy = new Leaf("DoNothingStrategy", new DoNothingStrategy(this));

        doNothingSequence.AddChild(doNothingCondition);
        doNothingSequence.AddChild(doNothingStrategy);

        inDeathSequence.AddChild(deathCondition);
        inDeathSequence.AddChild(inDeathStrategy);

        mainSelectorNode.AddChild(stayStillStrategy);
        mainSelectorNode.AddChild(chaseSequence);
        mainSelectorNode.AddChild(attackSequence);
        mainSelectorNode.AddChild(inDeathSequence);
        mainSelectorNode.AddChild(doNothingSequence);

        bossBehaviourTree.AddChild(mainSelectorNode);

    }

    private void FixedUpdate()
    {
        bossBehaviourTree.Process();
    }

    private void Update()
    {
        AnimatorStateInfo stateInfo = bossAnim.GetCurrentAnimatorStateInfo(0);
        AnimationController();
        SetDireaction();

        if(!stateInfo.IsName("attack") && !stateInfo.IsName("death"))
        {
            SetRotation();
        }

        if(currentHealth <= 0)
        {
            isDead = true; 
        }
        
        if (playerController.isDead)
        {
            chase = false;
        }
    }

    public override void OnSummon(Vector2 position, Boss parent, ParticleSystem summonParticle)
    {
        isVulnerable = false;
        parent.summonedBeings.Add(this);
        belongsTo = parent;
        summonParticle.transform.position = transform.position = parent.summonPosition.transform.position;
        summonParticle.Play();
    }

    public override void AnimationController()
    {
        bossAnim.SetBool("attack", attack);
        bossAnim.SetBool("death", isDead);
        bossAnim.SetBool("chase", chase);
    }

    public override float InflictDamage()
    {
        return 5f;
    }

    public override void InitBehaviourTree()
    {
        bossBehaviourTree = new BehaviourTree("NightNorne");
    }

    public override void OnDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        bossHealthBar.SetCurrentHealth(currentHealth);
    }

    public void OnDeath()
    {
        belongsTo.summonedBeings.Remove(this);
        GameManager.instance.blackBoard.UnRegisterEntry("NightBorne");
        Destroy(gameObject);
    }
}
