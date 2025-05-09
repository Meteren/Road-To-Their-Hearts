using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private ParticleSystem portalParticle;

    Collider2D portalColl;

    private void Start()
    {
        portalColl = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerController>(out PlayerController controller))
        {
            
            GameManager.instance.levelGenerator.currentLevel++;
            GameManager.instance.levelGenerator.bossLevel++;
            GameManager.instance.levelGenerator.currentLevelPart = 0;
            PlayerPrefs.SetInt("CurrentParkourLevel",GameManager.instance.levelGenerator.currentLevel == 3 ? 2 :
                GameManager.instance.levelGenerator.currentLevel);
            PlayerPrefs.SetInt("CurrentBossLevel", GameManager.instance.levelGenerator.bossLevel == 4 ? 3 :
                GameManager.instance.levelGenerator.bossLevel);
            PlayerPrefs.Save();
            controller.canMove = false;
            portalColl.enabled = false;
            if (GameManager.instance.levelGenerator.currentLevel == 3)
            {
                GameManager.instance.endGame.SetActive(true);
            }
            else
            {
                GameManager.instance.TransitionScenes(1);
                
            }
    
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
        portalColl.enabled = true;
    }

}
