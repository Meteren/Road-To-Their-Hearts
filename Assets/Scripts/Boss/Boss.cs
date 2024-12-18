using AdvancedStateHandling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : MonoBehaviour
{

    public GameObject panel;

    [Header("Dialogue States")]
    public bool specialOneProgressed = false;
    public bool specialTwoProgressed = false;
    public bool firstEncounterReady = true;
    public bool inSpecialOneToSayIsReady = false;
    public bool inSpecialTwoToSayIsReady = false;


    [Header("Dialogues")]
    [SerializeField] protected DialogueContainer firstEncounter;
    [SerializeField] protected DialogueContainer inSepcialOneToSay;
    [SerializeField] protected DialogueContainer inSpecialTwoToSay;
    [SerializeField] protected DialogueContainer inDeathToSay;
    [SerializeField] protected DialogueContainer inCharacterDeathToSay;

    [Header("Death Portal")]
    public Portal deathPortal;
    protected BlackBoard blackBoard => GameManager.instance.blackBoard;
    protected PlayerController playerController
        => blackBoard.GetValue("PlayerController", out PlayerController _controller) ? _controller : null;
    [SerializeField] protected LayerMask ground;
    [SerializeField] protected LayerMask bossLayer;
    public SpriteRenderer bossRenderer;

    public Animator bossAnim;

    public Vector2 direction;

    public float xVelocity;
    public float yVelocity;

    protected BehaviourTree bossBehaviourTree;
    protected AdvancedStateMachine dialogueStateMachine = new AdvancedStateMachine();

    public List<Boss> summonedBeings = new List<Boss>();
    public Transform summonPosition;
    public float distanceToPlayer => Vector2.Distance(transform.position, playerController.transform.position);

    [Header("Health")]
    public HealthBar bossHealthBar;
    public float currentHealth;
    public float maxHealth = 100;

    [Header("Rb")]
    public Rigidbody2D rb;

    [Header("Conditions")]
    public bool isDead = false;
    public bool isFacingLeft = true;
    public bool isInCloseRangeAttack = false;
    public bool canAvatarDie = false;
    public bool isInLongRangeAttack = false;
    public bool inDeathProgressed = false;
    public bool inDeathToSayIsReady = false;
    public bool inCharacterDeathToSayIsReady = false;
    public bool inCharacterDeathProgressed = false;
    public bool phaseTwo;
    public bool invincible = false;
    public bool chase;
    public bool phaseTwoIdle = false;
    public bool inPhaseTwo = false;
    public bool isVulnerable = true;

    [Header("Portal Spawn Point")]
    public Transform portalSpawnPoint;

    public abstract void OnDamage(float damageAmount);

    public abstract float InflictDamage();

    public abstract void InitBehaviourTree();

    public abstract void AnimationController();

    protected void IgnoreCollision()
    {
        Physics2D.IgnoreLayerCollision(playerController.gameObject.layer, gameObject.layer);
    }

    protected void SetRotation()
    {
        if (direction.x < 0 && !isFacingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            isFacingLeft = true;
        }

        if (direction.x > 0 && isFacingLeft)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            isFacingLeft = false;
        }
    }

    public virtual void OnSummon(Vector2 position, Boss parent, ParticleSystem summonParticle)
    {
        return;
    }

    public void SetDireaction()
    {
        direction = (playerController.transform.position - transform.position).normalized;

    }
    public void AfterDeathEvent()
    {
        Color color = bossRenderer.color;

        color.a = 0;

        bossRenderer.color = color;
    }
}
