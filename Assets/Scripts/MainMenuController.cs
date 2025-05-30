using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Texture2D mouseTexture;
    [SerializeField] private Button continueButton;
    // Start is called before the first frame update
    [SerializeField] private Animator transitAnimator;
    [SerializeField] private Animator initTransitAnimator;
    private bool start = false;

    Vector2 mousePosition = Vector2.zero;

    private void Start()
    {
        Cursor.SetCursor(mouseTexture, mousePosition, CursorMode.Auto);
        Cursor.visible = true;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Color color = continueButton.GetComponent<Image>().color;

        if (PlayerPrefs.GetInt("CurrentParkourLevel") != 0)
        {    
            color.a = 1;
            continueButton.interactable = true;
        }
        else
        {
            
            color.a = 0.3f;
            continueButton.interactable = false;

        }
        continueButton.GetComponent<Image>().color = color;
    }

    private void Update()
    {
        AnimatorStateInfo animatorStateInfo = initTransitAnimator.GetCurrentAnimatorStateInfo(0);
        if(animatorStateInfo.normalizedTime >=1)
        {
            initTransitAnimator.gameObject.SetActive(false);
        }
        if (!Cursor.visible)
        {
            Cursor.visible = true;
        }
    }
    public void OnClickStart()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        TransitToParkourScene();

    }

    public void OnClickContinue()
    {
        TransitToParkourScene();
    }

    public void OnClickExit()
    {
        StartCoroutine(ExitGame());
    }

    private void TransitToParkourScene()
    {
        StartCoroutine(StartTransition());
    }


    private IEnumerator StartTransition()
    {
        transitAnimator.gameObject.SetActive(true);
        start = true;
        transitAnimator.SetBool("start", start);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync(1);
    }

    private IEnumerator ExitGame()
    {
        transitAnimator.gameObject.SetActive(true);
        start = true;
        transitAnimator.SetBool("start", start);
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }  
    

}
