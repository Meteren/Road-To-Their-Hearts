using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSpike : MainTrap, ITrap
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() != null)
        {
            TrapLogic();
        }
    }

    public void TrapLogic()
    {
        controller.knockBack = true;
    }
}
