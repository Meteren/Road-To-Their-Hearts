using UnityEngine;

public class AttackFrame : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            if (controller.isInDash)
            {
                Debug.Log("Avoided!!!");
            }
            else
            {
                if(controller.stateMachine.currentState is not DamageState)
                {
                    Debug.Log("Hit");
                    Boss boss = GetComponentInParent<Boss>() ?? GameObject.Find("BossX").GetComponent<Boss>();
                    controller.isDamaged = true;
                    controller.OnDamage(boss.InflictDamage());
                    controller.damageDirection = boss.direction;
                }
                
            }


        }
        else
        {
            Debug.Log("Player Null");
        }
    }
}
