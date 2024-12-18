using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingleTon<GameManager>
{
    public BlackBoard blackBoard = new BlackBoard();
    [SerializeField] private FireBall objectToInstantiate;
    [SerializeField] private HitParticle hitParticle;
    public DashBar dashBar;
    [SerializeField] private Animator sceneTransitioning;
    public GenerateLevels levelGenerator;
    public bool init = false;
    public MainCamera mainCam;
    PlayerController playerController => 
        blackBoard.GetValue("PlayerController",out PlayerController _controller) ? _controller : null;

   
    private void Start()
    {
        SceneManager.sceneLoaded += ReadyInsantiations;
        ObjectPooling.SetupPool("FireBall", objectToInstantiate);
        ObjectPooling.SetupPool("HitParticle", hitParticle);
    }
    private void Update()
    {
        Debug.Log("Current Level: " + levelGenerator.currentLevel);
    }

    private void ReadyInsantiations(Scene scene, LoadSceneMode mode)
    {      
        if(scene.buildIndex != 0)
        {   
            
            ObjectPooling.SetupPool("FireBall", objectToInstantiate);
            ObjectPooling.SetupPool("HitParticle", hitParticle);
            blackBoard.GetValue("PlayerController", out PlayerController _controller);
            _controller.transform.position = GameObject.Find("PlayerPosition").transform.position;
            dashBar = GameObject.Find("Canvas").transform.Find("DashBar").GetComponent<DashBar>();
            playerController.playerHealthBar = GameObject.Find("Canvas").transform.Find("Player HealthBar").GetComponent<PlayerHealthBar>();
            playerController.playerHealthBar.SetMaxHealth(playerController.maxHealth);
        }
        else
        {
            levelGenerator.DeleteGeneratedLevels(true);
            playerController.currentHealth = 100;
            playerController.playerHealthBar.SetCurrentHealth(playerController.currentHealth);
            playerController.isFacingRight = true;
            GameManager.instance.blackBoard.UnRegisterEntry("PostProcessVolume");
            GameManager.instance.blackBoard.UnRegisterEntry("Channel");
            mainCam.GetComponent<CinemachineConfiner>().m_BoundingShape2D =
               GameObject.Find("Background").transform.Find("6").GetComponent<PolygonCollider2D>();
            playerController.transform.rotation = Quaternion.Euler(0, 0, 0);
            levelGenerator.OnLevelStart();
            playerController.transform.position = levelGenerator.levels[levelGenerator.currentLevel].levelParts[0].Find("StartPoint").transform.position;
            
        }
     
    }

    public void TransitionScenes(int buildIndex)
    {
        StartCoroutine(StartTransitioning(buildIndex));
    }

    private IEnumerator StartTransitioning(int buildIndex)
    {
        sceneTransitioning.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync(buildIndex);
        sceneTransitioning.SetTrigger("Start");
        
    }

  
    

}
