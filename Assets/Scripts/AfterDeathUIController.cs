using UnityEngine;
using UnityEngine.UI;

public class AfterDeathUIController : MonoBehaviour
{
    [SerializeField] private Animator onDeath;
    [SerializeField] private Button restart;
    [SerializeField] private Button mainMenu;
    private GameObject bossCamera => GameObject.Find("Camera");
    public void OnClickRestart()
    {
        GameManager.instance.isRestarted = true;
        GameManager.instance.TransitionScenes(1,bossCamera);
        DeactivateButtons();
            
    }

    public void OnClickMainMenu()
    {
        GameManager.instance.TransitionScenes(0);
        GameManager.instance.blackBoard.UnRegisterEntry("PlayerController");
        DeactivateButtons();
    }

    private void Update()
    {
        AnimatorStateInfo animatorStateInfo = onDeath.GetCurrentAnimatorStateInfo(0);

        if(animatorStateInfo.normalizedTime >= 1)
        {
            restart.gameObject.SetActive(true);
            mainMenu.gameObject.SetActive(true);
        }
    }

    private void DeactivateButtons()
    {
        restart.interactable = false;
        mainMenu.interactable = false;
    }
}
