using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private ParticleSystem portalParticle;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            GameManager.instance.levelGenerator.currentLevel++;
            GameManager.instance.levelGenerator.currentLevelPart = 0;
            GameManager.instance.TransitionScenes(0);  
        }
    }

    public void OnSpawn()
    {
        float duration = portalParticle.main.duration;
        StartCoroutine(ActivateTrigger(duration));

    }

    private IEnumerator ActivateTrigger(float duration)
    {
        yield return new WaitForSeconds(duration);
        GetComponent<Collider2D>().enabled = true;
    }

}
