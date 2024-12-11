using UnityEngine;
using AdvancedStateHandling;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;
using Cinemachine;

public class JumpState : BasePlayerState
{
    public JumpState(PlayerController controller) : base(controller)
    {
    }

    public override void OnStart()
    {
        base.OnStart();
        controller.rb.velocity = new Vector2(controller.rb.velocity.x, controller.jumpForce);
        Debug.Log("JumpStarted");
    }
    public override void OnExit()
    {
        base.OnExit();
        Debug.Log("JumpStateExited");
        controller.jump = false;
    }

    public override void Update()
    {

        base.Update();
        Debug.Log("JumpStateUpdate");
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            controller.rb.velocity = new Vector2(controller.xAxis * controller.charSpeed, controller.rb.velocity.y);
        }
        else
        {
            controller.rb.velocity = new Vector2(moveSpeed, controller.rb.velocity.y);
        }
        
        if(Input.GetKeyDown(KeyCode.Space))
            controller.doubleJumped = true;
        if(Input.GetKeyDown(KeyCode.LeftShift) && SceneManager.GetActiveScene().buildIndex != 0 && !controller.dashInCoolDown)
            controller.isInDash = true;
        if (Input.GetKeyDown(KeyCode.V) && controller.interaction)
            controller.canAttack = true;
      
    }
}

public class MoveState : BasePlayerState
{
    private float xInput;
    public MoveState(
        PlayerController controller) : base(controller) { }

    public override void OnStart()
    {
        base.OnStart();
        controller.doubleJumped = false;
        controller.controlDoubleJumpAnim = false;
        
    }
    public override void OnExit()
    {
        base.OnExit();
       
    }

    public override void Update()
    {
        base.Update();
        xInput = Input.GetAxisRaw("Horizontal");
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            controller.rb.velocity = new Vector2(controller.xAxis * controller.charSpeed, controller.rb.velocity.y);
        }
        else
        {
            controller.rb.velocity = new Vector2(moveSpeed, controller.rb.velocity.y);
        }
        
        Debug.Log("MoveStateUpdate");
        if (Input.GetKeyDown(KeyCode.Space) && !controller.isJumped)
        {
            controller.jump = true;

        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(SceneManager.GetActiveScene().buildIndex == 0)
                controller.isSliding = true;
            if(SceneManager.GetActiveScene().buildIndex != 0 && !controller.dashInCoolDown)
            {
                controller.isInDash = true;
                controller.dashParticles.Play();
            }
        }
        if (Input.GetKeyDown(KeyCode.V) && controller.interaction)
            controller.canAttack = true;


    }

}
public class SlideState : BasePlayerState
{

    float timer = 1f;

    public SlideState(PlayerController controller) : base(controller)
    {
    }

    public override void OnStart()
    {
        base.OnStart();
        controller.rb.velocity = new Vector2(controller.slideForce * controller.xAxis, controller.rb.velocity.y);   
        
    }
    public override void OnExit()
    {
        base.OnExit();
        controller.isSliding = false;
        timer = 1f;  
    }

    public override void Update()
    {
        Debug.Log("Slide State");
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            if (controller.freeToGetUp)
            {
                controller.isSliding = false;
                controller.getUpAnimController = true;

            }
            else
            {
                timer = -1;

            }
           
            
        }
  
       
    }

}

public class FallState : BasePlayerState
{
    public FallState(PlayerController controller) : base(controller)
    {
        
    }

    public override void OnStart()
    {
        base.OnStart();
        
    }

    public override void OnExit()
    {
        base.OnExit();
        
    }

    public override void Update()
    {
        base.Update();
        Debug.Log("Fall State Update");
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            controller.rb.velocity = new Vector2(controller.xAxis * controller.charSpeed, controller.rb.velocity.y);
        }
        else
        {
            controller.rb.velocity = new Vector2(moveSpeed, controller.rb.velocity.y);
        }
        

        if (Input.GetKeyDown(KeyCode.LeftShift) && SceneManager.GetActiveScene().buildIndex != 0 && !controller.dashInCoolDown)
        {
            controller.isInDash = true;
        }

        if (Input.GetKeyDown(KeyCode.V) && controller.interaction)
            controller.canAttack = true;

        if (Input.GetKeyDown(KeyCode.Space) && !controller.doubleJumped)
            controller.doubleJumped = true;
        CheckHighness();

        
    }
}

public class LedgeGrabState : BasePlayerState
{
    Vector2 standartOffset1;
    Vector2 standartOffset2;
    public LedgeGrabState(PlayerController controller) : base(controller)
    {
        standartOffset1 = controller.offSet1;
        standartOffset2 = controller.offSet2;
    }
    

    public override void OnStart()
    {
        base.OnStart();
        SetLedgeClimbDirection();
        HandleLedgePosition();
        Debug.Log("LedgeGrab Start");
        controller.canRoll = false;

    }

    public override void OnExit()
    {
        base.OnExit();
        controller.rb.gravityScale = 2;
        controller.ledgeDetected = false;
        Debug.Log("LedgeGrab Exit");
    }

    public override void Update()
    {
        Debug.Log("LedgeGrab Update");
    }

    private void HandleLedgePosition()
    {
        Vector2 refPos = controller.playerFrame.transform.position;
        controller.startOfLedgGrabPos = refPos + controller.offSet2;
        controller.endOfLedgeGrabPos = refPos + controller.offSet1;
        controller.rb.gravityScale = 0;
        controller.rb.velocity = new Vector2(controller.rb.velocity.x, 0);
        controller.transform.position = controller.startOfLedgGrabPos;
    }

    private void SetLedgeClimbDirection()
    {
        controller.offSet1 = standartOffset1;
        controller.offSet2 = standartOffset2;

        if (controller.isFacingRight)
        {
            return;
        }

        if (!controller.isFacingRight)
        {
            controller.offSet1 = new Vector2(-1 * controller.offSet1.x, controller.offSet1.y);
            controller.offSet2 = new Vector2(-1 * controller.offSet2.x, controller.offSet2.y);
        }

    }
}

public class DoubleJumpState : BasePlayerState
{
    public DoubleJumpState(PlayerController controller) : base(controller)
    {
    }

    public override void OnStart()
    {
        base.OnStart();
        controller.rb.velocity = new Vector2(controller.rb.velocity.x, controller.doubleJumpForce);
        controller.controlDoubleJumpAnim = true;
        controller.canRoll = false;

    }

    public override void OnExit()
    {
        base.OnExit();

    }

    public override void Update()
    {
        base.Update();
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            controller.rb.velocity = new Vector2(controller.xAxis * controller.charSpeed, controller.rb.velocity.y);
        }
        else
        {
            controller.rb.velocity = new Vector2(moveSpeed, controller.rb.velocity.y);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && SceneManager.GetActiveScene().buildIndex != 0 && !controller.dashInCoolDown)
        {
            controller.isInDash = true;
        }
        if (Input.GetKeyDown(KeyCode.V) && controller.interaction)
            controller.canAttack = true;
    }

}

public class AfterDoubleJumpFallState : BasePlayerState
{

    public AfterDoubleJumpFallState(PlayerController controller) : base(controller)
    {
       
    }

    public override void OnStart()
    {
        base.OnStart();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        base.Update();
        CheckHighness();
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            controller.rb.velocity = new Vector2(controller.xAxis * controller.charSpeed, controller.rb.velocity.y);
        }
        else
        {
            controller.rb.velocity = new Vector2(moveSpeed, controller.rb.velocity.y);
        }
       
        if (Input.GetKeyDown(KeyCode.LeftShift) && SceneManager.GetActiveScene().buildIndex != 0 && !controller.dashInCoolDown)
        {
            controller.isInDash = true;
        }

        if (Input.GetKeyDown(KeyCode.V) && controller.interaction)
            controller.canAttack = true;

    }

}

public class RollState : BasePlayerState
{
    float rollForce = 7f;
    public RollState(PlayerController controller) : base(controller) { }


    public override void OnStart()
    {
        base.OnStart();
        controller.rb.velocity = Vector2.zero;
        controller.rb.velocity = new Vector2(rollForce * direction, controller.rb.velocity.y);
        Debug.Log("Entered roll state");
        Time.timeScale = 0.3f;
    }

    public override void OnExit()
    {
        base.OnExit();
        Debug.Log("Exited roll state");
        Time.timeScale = 1f;
        controller.canRoll = false;
    }

    public override void Update()
    {
        //base.Update();
        Debug.Log("RollState update");
    }
}

public class DashState : BasePlayerState
{
    float dashSpeed = 13f;
    float timeToStayInDash = 0.3f;
    PostProcessVolume volume => 
        GameManager.instance.blackBoard.GetValue("PostProcessVolume", out PostProcessVolume _volume) ? _volume : null;
    ChromaticAberration cAberration => volume.profile.TryGetSettings(out ChromaticAberration _cAberration) ? _cAberration : null;
    public DashState(PlayerController controller) : base(controller)
    {

    }
    public override void OnStart()
    {
        base.OnStart();
        GameManager.instance.dashBar.SetDashBarToZero();
        controller.rb.velocity = new Vector2(controller.rb.velocity.x, 0);
        controller.rb.velocity = new Vector2(direction * dashSpeed, controller.rb.velocity.y);
        controller.controlDoubleJumpAnim = false;
        Time.timeScale = 0.3f;
        cAberration.active = true;
        if(!controller.isJumped && SceneManager.GetActiveScene().buildIndex == 1)
            controller.dashParticles.Play();
    }
    public override void OnExit()
    {
        base.OnExit();
        controller.dashInCoolDown = true;
        if (controller.dashParticles.isPlaying && SceneManager.GetActiveScene().buildIndex == 1)
            controller.dashParticles.Stop();
    }

    public override void Update()
    {
        Debug.Log("DashState");
        timeToStayInDash -= Time.deltaTime;
        if (timeToStayInDash <= 0.2f && timeToStayInDash > 0f)
        {
            Time.timeScale = 1f;
            cAberration.active = false;
        }
            
        if(timeToStayInDash <= 0f)
        {
            controller.isInDash = false;
            timeToStayInDash = 0.3f;
        }
        
        
    }
 
}

public class DamageState : BasePlayerState
{
    float yForce = 7f;
    float xForce = 7f;
    float timer = 0.1f;
    public DamageState(PlayerController controller) : base(controller)
    {
               
    }

    public override void OnStart()
    {
        Debug.Log("Damage Start");
        base.OnStart();
        controller.rb.velocity = Vector2.zero;
        controller.rb.AddForce(new Vector2(controller.damageDirection.x * xForce, controller.damageDirection.x == 0 ? 2f : yForce), ForceMode2D.Impulse);
        controller.StartCoroutine(TimeToStay());
        controller.StartCoroutine(SlowTime());
    }

    public override void OnExit()
    {
        base.OnExit();
        controller.rb.velocity = new Vector2(0,0);
        Time.timeScale = 1f;
        ResetColor();
    }

    public override void Update()
    {
        Debug.Log("Damage State");
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            Color color = GetColor();
            if(color.a == 30f / 255f)
            {
                color.a = 1f;
            }
            else
            {
                color.a = 30f / 255f;
            } 
            controller.playerRenderer.color = color;
            timer = 0.1f;
        }
        
        
    }

    private IEnumerator TimeToStay()
    {
        yield return new WaitForSeconds(0.15f);
        controller.isDamaged = false;
    }

    private IEnumerator SlowTime()
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSeconds(0.05f);
        Time.timeScale = 1f;
    }

    private Color GetColor()
    {
        return controller.playerRenderer.color;
    }

    private void ResetColor()
    {
        Color color = GetColor();
        color.a = 1f;
        controller.playerRenderer.color = color;
    }

}

public class DeathState : BasePlayerState
{
    bool blockCoroutine = false;
  
    float airForce = 10f;
    float groundForce = 5f;
    public DeathState(PlayerController controller) : base(controller)
    {
    }
   
    public override void OnStart()
    {
        base.OnStart();
        Debug.Log("Welcome to afterlife");
        controller.rb.velocity = new Vector2(0, 0);
        if(controller.isJumped)
        {
            controller.rb.AddForce(new Vector2(controller.damageDirection.x * airForce, controller.rb.velocity.y),ForceMode2D.Impulse);
        }
        else
        {
            controller.rb.AddForce(new Vector2(controller.damageDirection.x * groundForce, controller.rb.velocity.y),ForceMode2D.Impulse);
            controller.StartCoroutine(GroundImpact());
        }
       
    }

    public override void OnExit()
    {
        base.OnExit();
        //You can't go back. Forget about exiting this state. In the end you are dead after all :D
    }


    public override void Update()
    {
        if (!controller.isJumped && !blockCoroutine)
        {   
            controller.StartCoroutine(GroundImpact());
            blockCoroutine = true;
        }
        Debug.Log("You are dead");
    }

    private IEnumerator GroundImpact()
    {
        Debug.Log("GroundImpact");
        controller.rb.AddForce(new Vector2(controller.damageDirection.x * groundForce, controller.rb.velocity.y), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.1f);
        controller.rb.velocity = Vector2.zero;
    }
}

public class AfterStepOnJumperState : BasePlayerState
{
    Vector2 force = new Vector2(20, 10);
    float timer = 1f;
    public AfterStepOnJumperState(PlayerController controller) : base(controller)
    {
    }

    public override void OnStart()
    {
        Debug.Log("AfterJumper");
        base.OnStart();
        controller.rb.velocity = Vector2.zero;
        controller.rb.AddForce(force, ForceMode2D.Impulse);
    }

    public override void OnExit()
    {
        base.OnExit();
        timer = 1f;
        controller.isOnJumper = false;
        controller.canRoll = true;
    }


    public override void Update()
    {
        //base.Update();
        Debug.Log("Update of AfterJumper");
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            timer = 1f;
            controller.isOnJumper = false;
        }
    }
}

public class KnockBackState : BasePlayerState
{
    float knockBackForce = 3f;
    float secondKnockBackForce = 5f;
    bool knockBackInitialized = false;
    bool knockBackInTheAirInitialized = false;
    CinemachineBasicMultiChannelPerlin channel;
        
    public KnockBackState(PlayerController controller) : base(controller)
    {
        channel =
            GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>()
            .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public override void OnStart()
    {
        base.OnStart();
        Debug.Log("Knockback Started");
        controller.rb.velocity = Vector2.zero;
        if (controller.isJumped)
        {
            controller.rb.AddForce(new Vector2(knockBackForce * (-1 * controller.xAxis), controller.rb.velocity.y), ForceMode2D.Impulse);
            knockBackInTheAirInitialized = true;
            
        }
            
            
        if (!controller.isJumped)
            controller.rb.AddForce(new Vector2(secondKnockBackForce * (-1 * controller.xAxis), controller.rb.velocity.y), ForceMode2D.Impulse);
        
        controller.StartCoroutine(ShakePeriod());
    }

    public override void OnExit()
    {
        base.OnExit();
        controller.rb.velocity = Vector2.zero;
        knockBackInitialized = false;
        knockBackInTheAirInitialized = false;
    }

    public override void Update()
    {
        //base.Update();
        Debug.Log("KnockBack Update");
        if (!controller.isJumped && !knockBackInitialized && knockBackInTheAirInitialized)
        {
            knockBackInitialized = true;
            controller.rb.velocity = Vector2.zero;
            controller.StartCoroutine(WaitOnTheGround());
            
        }
    }

    private IEnumerator WaitOnTheGround()
    {
        controller.rb.AddForce(new Vector2(secondKnockBackForce * (-1 * controller.xAxis), controller.rb.velocity.y), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        controller.rb.velocity = Vector2.zero;
        controller.getUpAnimController = true;
    }

    private IEnumerator ShakePeriod()
    {
        channel.m_AmplitudeGain = 2.5f;
        yield return new WaitForSeconds(0.3f);
        channel.m_AmplitudeGain = 0f;
    }

}

public class StayStillState : BasePlayerState
{
    public StayStillState(PlayerController controller) : base(controller)
    {
    }


    public override void OnStart()
    {
        base.OnStart();
        controller.rb.velocity = Vector2.zero;
        controller.xAxis = 0f;
    }


    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
        //base.Update();
        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            controller.canMove = true;
        }
    }
}

public class AttackState : BasePlayerState
{
    AnimatorStateInfo animatorStateInfo;
    public AttackState(PlayerController controller) : base(controller)
    {
    }

    public override void OnStart()
    {
        base.OnStart();
        controller.rb.velocity = new Vector2(0, controller.rb.velocity.y);
    }

    public override void OnExit()
    {
        controller.canAttack = false;
        base.OnExit();
    }

    public override void Update()
    {
        //base.Update();
        animatorStateInfo = controller.playerAnimController.GetCurrentAnimatorStateInfo(0);

        if (animatorStateInfo.IsName("combat"))
            if (animatorStateInfo.normalizedTime >= 1)
                controller.canAttack = false;
    }
}






