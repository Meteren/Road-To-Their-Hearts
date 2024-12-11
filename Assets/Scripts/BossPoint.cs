using UnityEngine;

public class BossPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() != null)
        {
            GameManager.instance.TransitionScenes(GameManager.instance.levelGenerator.currentLevel + 1);
            collision.gameObject.GetComponent<PlayerController>().canMove = false;
        }
    }

}
