using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : SingleTon<GameManager>
{
    public bool isRestarted = false;
    public bool isPaused = false;
    public BlackBoard blackBoard = new BlackBoard();
    [SerializeField] private FireBall objectToInstantiate;
    [SerializeField] private HitParticle hitParticle;
    public DashBar dashBar;
    [SerializeField] private Animator sceneTransitioning;
    
    public GenerateLevels levelGenerator;
    public bool init = false;
    public MainCamera mainCam => GameObject.Find("Virtual Camera").GetComponent<MainCamera>();
    PlayerController playerController => 
        blackBoard.GetValue("PlayerController",out PlayerController _controller) ? _controller : null;

    [Header("UI Objects")]
    public GameObject afterDeathUI;
    public GameObject pauseMenu;
    public GameObject endGame;

    [Header("Parkour Cam")]
    public GameObject deathPlace;

    [Header("Pause Menu Buttons")]
    public RectTransform restartButtonSize;
    public RectTransform mainMenuButtonSize;
    public TextMeshProUGUI textRestart;
    public TextMeshProUGUI textMainMenu;

    [Header("Audio Manager")]
    public AudioManager audioManager;

    [Header("Mouse Cursor")]
    public Texture2D UIMouseCursor;
    public Texture2D shootMouseCursor;


    float x, y,fontSize;

    GameObject dialogueBox;

    private void Start()
    {
        SceneManager.sceneLoaded += ReadyInsantiations;
        AudioListener.pause = false;
        Cursor.visible = false;
        Debug.Log("OnStart");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
 
            if (isPaused)
            {
                if (dialogueBox != null)
                {
                    dialogueBox.SetActive(true);
                }
                restartButtonSize.sizeDelta = new Vector2(x, y);
                mainMenuButtonSize.sizeDelta = new Vector2(x, y);
                textRestart.fontSize = fontSize;
                textMainMenu.fontSize = fontSize;
                Time.timeScale = 1.0f;
                pauseMenu.SetActive(false);
                isPaused = false;
                AudioListener.pause = false;
                

            }
            else
            {
                dialogueBox = GameObject.Find("Dailogue Box");
                if(dialogueBox != null)
                {
                    dialogueBox.SetActive(false);
                }
                x = restartButtonSize.sizeDelta.x;
                y = restartButtonSize.sizeDelta.y;
                fontSize = textRestart.fontSize;
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
                isPaused = true;
                AudioListener.pause = true;

            }
        }

       
      
    }

    public void ReadyInsantiations(Scene scene, LoadSceneMode mode)
    {      
        if(scene.buildIndex != 1 && scene.buildIndex != 0)
        {
            deathPlace.gameObject.SetActive(false);
            ObjectPooling.SetupPool("FireBall", objectToInstantiate);
            ObjectPooling.SetupPool("HitParticle", hitParticle);
            blackBoard.GetValue("PlayerController", out PlayerController _controller);
            _controller.transform.position = GameObject.Find("PlayerPosition").transform.position;
            dashBar = GameObject.Find("Canvas").transform.Find("DashBar").GetComponent<DashBar>();
            playerController.playerHealthBar = GameObject.Find("Canvas").transform.Find("Player HealthBar").GetComponent<PlayerHealthBar>();
            playerController.currentHealth = 100;
            playerController.playerHealthBar.SetMaxHealth(playerController.maxHealth);
            Cursor.SetCursor(GameManager.instance.shootMouseCursor, Vector2.zero, CursorMode.Auto);
            Cursor.visible = true;

        }
        else if(scene.buildIndex == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            deathPlace.gameObject.SetActive(true);
            levelGenerator.currentLevelPart = 0;
            levelGenerator.DeleteGeneratedLevels(true);        
            playerController.isFacingRight = true;
            GameManager.instance.blackBoard.UnRegisterEntry("PostProcessVolume");
            GameManager.instance.blackBoard.UnRegisterEntry("Channel");
            mainCam.GetComponent<CinemachineConfiner>().m_BoundingShape2D =
            GameObject.Find("Background").transform.Find("6").GetComponent<PolygonCollider2D>();
            playerController.transform.rotation = Quaternion.Euler(0, 0, 0);
            levelGenerator.OnLevelStart();
            playerController.transform.position = levelGenerator.levels[levelGenerator.currentLevel].levelParts[0].Find("StartPoint").transform.position;
            playerController.rb.velocity = Vector2.zero;
            Cursor.visible = false;
        }
        
    }

    public void TransitionScenes(int buildIndex,GameObject bossCamera = null)
    {
        StartCoroutine(StartTransitioning(buildIndex,bossCamera));
    }

    private IEnumerator StartTransitioning(int buildIndex, GameObject bossCamera)
    {
        sceneTransitioning.SetTrigger("End");
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadSceneAsync(buildIndex);
        if (isRestarted)
        {
            if(bossCamera != null)
            {
                bossCamera.SetActive(false);
            }  
            Destroy(gameObject);
            
        }
        else
        {
            sceneTransitioning.SetTrigger("Start");
        }
        Time.timeScale = 1f;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= ReadyInsantiations;
    }

}
