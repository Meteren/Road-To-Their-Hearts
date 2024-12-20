using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlace : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            controller.canMove = false;
            GameManager.instance.afterDeathUI.SetActive(true);
        }
    }

   
}
