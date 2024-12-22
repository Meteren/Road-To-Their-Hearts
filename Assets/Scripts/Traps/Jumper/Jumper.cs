using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MainTrap, ITrap
{
    Animator jumperAnimator;
    [SerializeField] private Transform left;
    [SerializeField] private Transform right;
 
    void Start()
    {
        jumperAnimator = GetComponent<Animator>();    
    }

    public void TrapLogic()
    {  
        controller.isOnJumper = true;
        GameManager.instance.audioManager.PlaySFX(6);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            if(controller.transform.position.x > left.position.x && controller.transform.position.x < right.position.x)
                jumperAnimator.SetTrigger("triggerJumper");
            
        }
    }

   
}
